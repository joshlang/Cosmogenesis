﻿using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Cosmogenesis.Core;

public abstract class DbBase
{
    internal const int ReadIdsQueryThreshhold = 4;
    internal const int MaxIdsPerQuery = 10000;
    internal const int MaxIdLengthPerQuery = 500000;
    internal protected virtual Container Container { get; } = default!;
    internal protected virtual DbSerializerBase Serializer { get; } = default!;

    public virtual bool IsReadOnly { get; }
    public bool ValidateStateBeforeSave { get; }
    bool IsWarm;

    protected DbBase() { }

    protected DbBase(
        Container container,
        DbSerializerBase serializer,
        bool isReadOnly,
        bool validateStateBeforeSave)
    {

        Container = container ?? throw new ArgumentNullException(nameof(container));
        Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        IsReadOnly = isReadOnly;
        ValidateStateBeforeSave = validateStateBeforeSave;
    }

    public async virtual Task ValidateContainerAsync(CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("select * from c where c.id=@containerId")
            .WithParameter("@containerId", Container.Id);
        var containers = Container
            .Database
            .GetContainerQueryIterator<ContainerProperties>(query);
        while (containers.HasMoreResults)
        {
            var prop = await containers.ReadNextAsync(cancellationToken).ConfigureAwait(false);
            var settings = prop.Resource.FirstOrDefault();
            if (settings is null)
            {
                continue;
            }
            if (settings.PartitionKeyDefinitionVersion == PartitionKeyDefinitionVersion.V1)
            {
                throw new InvalidOperationException("The container uses the old partition key system; upgrade to V2");
            }
            if (settings.PartitionKeyPath != "/pk")
            {
                throw new InvalidOperationException($"The container's partition key path should be '/pk' but instead is '{settings.PartitionKeyPath}'");
            }
            return;
        }
        throw new InvalidOperationException("The container does not yet seem to exist");
    }

    public async virtual Task WarmupAsync(CancellationToken cancellationToken = default)
    {
        if (IsWarm)
        {
            return;
        }
        using var result = await Container.ReadItemStreamAsync(
            id: Guid.NewGuid().ToString(),
            partitionKey: new PartitionKey(Guid.NewGuid().ToString()),
            cancellationToken: cancellationToken).ConfigureAwait(false);
        IsWarm = true;
    }

    internal void ThrowIfReadOnly()
    {
        if (IsReadOnly)
        {
            throw new InvalidOperationException($"This database instance is in read-only mode");
        }
    }

    protected virtual async Task<T?> ReadByIdAsync<T>(string id, PartitionKey partitionKey, string type) where T : DbDoc
    {
        if (id is null)
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        using var result = await Container.ReadItemStreamAsync(
            id: id,
            partitionKey: partitionKey).ConfigureAwait(false);
        if (result.IsSuccessStatusCode)
        {
            var item = Serializer.FromStream<T>(result.Content) ?? throw new DbUnexpectedStateException("FromStream<T> returned null");
            Debug.Assert(item.id == id);
            Debug.Assert(partitionKey.ToString() == new PartitionKey(item.pk).ToString());
            Debug.Assert(!string.IsNullOrEmpty(item._etag));
            Debug.Assert(item._ts > 0);
            if (item.Type != type)
            {
                throw new InvalidOperationException($"This document is of type {item.Type}, not the expected type {type}");
            }
            return item;
        }
        if (result.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        throw result.ExceptionFromErrorStatus();
    }

    protected virtual async Task<T?[]> ReadByIdsAsync<T>(IEnumerable<string> ids, PartitionKey partitionKey, string type) where T : DbDoc
    {
        if (ids is null)
        {
            throw new ArgumentNullException(nameof(ids));
        }
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var idList = ids.Select(x => x ?? throw new ArgumentNullException(nameof(ids))).ToArray() ?? throw new ArgumentNullException(nameof(ids));
        if (idList.Length == 0)
        {
            return Array.Empty<T?>();
        }
        if (idList.Length <= ReadIdsQueryThreshhold)
        {
            var readTasks = idList.Select(id => ReadByIdAsync<T>(id: id, partitionKey: partitionKey, type: type)).ToList();
            await Task.WhenAll(readTasks).ConfigureAwait(false);
            return readTasks.Select(x => x.Result).ToArray();
        }

        var results = new T?[idList.Length];
        var resultMap = idList.Select((id, index) => (id, index)).ToDictionary(x => x.id, x => x.index);

        foreach (var queryIds in idList.Segment(MaxIdsPerQuery))
        {
            var totalLength = 0;
            var queryNowIds = queryIds
                .TakeWhile(x => (totalLength += x.Length) < MaxIdLengthPerQuery)
                .ToList();

            var query = Container.GetItemLinqQueryable<T>(
                allowSynchronousQueryExecution: false,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = partitionKey
                })
                .Where(x => queryNowIds.Contains(x.id));

            await foreach (var item in ExecuteQueryAsync(query))
            {
                Debug.Assert(partitionKey.ToString() == new PartitionKey(item.pk).ToString());
                Debug.Assert(!string.IsNullOrEmpty(item._etag));
                Debug.Assert(item._ts > 0);
                if (item.Type != type)
                {
                    throw new InvalidOperationException($"This document is of type {item.Type}, not the expected type {type}");
                }
                results[resultMap[item.id]] = item;
            }
        }

        return results;
    }

    internal protected virtual async Task<CreateResult<T>> CreateItemAsync<T>(T item, string type, PartitionKey partitionKey, string partitionKeyString) where T : DbDoc
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        if (partitionKeyString is null)
        {
            throw new ArgumentNullException(nameof(partitionKeyString));
        }

        ThrowIfReadOnly();

        if (item.id is null)
        {
            throw new InvalidOperationException("The document .id property is missing");
        }
        if (item._etag != null)
        {
            throw new InvalidOperationException("The document already has an etag");
        }
        if (item.pk is null)
        {
            item.pk = partitionKeyString;
        }
        else if (item.pk != partitionKeyString)
        {
            throw new InvalidOperationException("The document .pk property does not match this partition key");
        }
        if (item.Type is null)
        {
            item.Type = type;
        }
        else if (item.Type != type)
        {
            throw new InvalidOperationException($"The document .Type property does not match what was expected ({type})");
        }

        Debug.Assert(item.CreationDate == IsoDateCheater.MinValue, "Don't set CreationDate. It is overridden anyway.");
        item.CreationDate = DateTime.UtcNow;

        if (ValidateStateBeforeSave)
        {
            item.ValidateStateOrThrow();
        }

        using var payload = Serializer.ToStream(item);
        using var result = await Container.CreateItemStreamAsync(
            streamPayload: payload,
            partitionKey: partitionKey).ConfigureAwait(false);
        if (result.IsSuccessStatusCode)
        {
            var newItem = Serializer.FromStream<T>(result.Content) ?? throw new DbUnexpectedStateException("FromStream<T> returned null");
            Debug.Assert(item.id == newItem.id);
            Debug.Assert(item.pk == newItem.pk);
            Debug.Assert(item.Type == type);
            Debug.Assert(item._etag != newItem._etag);
            return new CreateResult<T>(newItem);
        }
        return result.CreateResultFromErrorStatus<T>();
    }

    internal protected virtual async Task<CreateOrReplaceResult<T>> CreateOrReplaceItemAsync<T>(T item, string type, PartitionKey partitionKey, string partitionKeyString) where T : DbDoc
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        if (partitionKeyString is null)
        {
            throw new ArgumentNullException(nameof(partitionKeyString));
        }

        ThrowIfReadOnly();

        if (item.id is null)
        {
            throw new InvalidOperationException("The document .id property is missing");
        }
        if (item._etag != null)
        {
            throw new InvalidOperationException("The document already has an etag");
        }
        if (item.pk is null)
        {
            item.pk = partitionKeyString;
        }
        else if (item.pk != partitionKeyString)
        {
            throw new InvalidOperationException("The document .pk property does not match this partition key");
        }
        if (item.Type is null)
        {
            item.Type = type;
        }
        else if (item.Type != type)
        {
            throw new InvalidOperationException($"The document .Type property does not match what was expected ({type})");
        }

        Debug.Assert(item.CreationDate == IsoDateCheater.MinValue, "Don't set CreationDate. It is overridden anyway.");
        item.CreationDate = DateTime.UtcNow;

        if (ValidateStateBeforeSave)
        {
            item.ValidateStateOrThrow();
        }

        using var payload = Serializer.ToStream(item);
        using var result = await Container.UpsertItemStreamAsync(
            streamPayload: payload,
            partitionKey: partitionKey).ConfigureAwait(false);
        if (result.IsSuccessStatusCode)
        {
            var newItem = Serializer.FromStream<T>(result.Content) ?? throw new DbUnexpectedStateException("FromStream<T> returned null");
            Debug.Assert(result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Created);
            Debug.Assert(item.id == newItem.id);
            Debug.Assert(item.pk == newItem.pk);
            Debug.Assert(item.Type == type);
            Debug.Assert(item._etag != newItem._etag);
            return new CreateOrReplaceResult<T>(
                document: newItem,
                alreadyExisted: result.StatusCode != HttpStatusCode.Created);
        }
        throw result.ExceptionFromErrorStatus();
    }

    internal protected virtual async Task<ReadOrCreateResult<T>> ReadOrCreateItemAsync<T>(T item, string type, PartitionKey partitionKey, string partitionKeyString, bool tryCreateFirst) where T : DbDoc
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        if (partitionKeyString is null)
        {
            throw new ArgumentNullException(nameof(partitionKeyString));
        }

        ThrowIfReadOnly();

        if (item.id is null)
        {
            throw new InvalidOperationException("The document .id property is missing");
        }
        if (item._etag != null)
        {
            throw new InvalidOperationException("The document already has an etag");
        }
        if (item.pk is null)
        {
            item.pk = partitionKeyString;
        }
        else if (item.pk != partitionKeyString)
        {
            throw new InvalidOperationException("The document .pk property does not match this partition key");
        }
        if (item.Type is null)
        {
            item.Type = type;
        }
        else if (item.Type != type)
        {
            throw new InvalidOperationException($"The document .type property does not match what was expected ({type})");
        }

        Debug.Assert(item.CreationDate == IsoDateCheater.MinValue, "Don't set CreationDate. It is overridden anyway.");
        item.CreationDate = DateTime.UtcNow;

        if (ValidateStateBeforeSave)
        {
            item.ValidateStateOrThrow();
        }

        const int ConcurrencyRetryCount = 10;
        for (var x = 0; x < ConcurrencyRetryCount; ++x)
        {
            if (tryCreateFirst)
            {
                using var payload = Serializer.ToStream(item);
                using var result = await Container.CreateItemStreamAsync(
                    streamPayload: payload,
                    partitionKey: partitionKey).ConfigureAwait(false);
                if (result.IsSuccessStatusCode)
                {
                    var newItem = Serializer.FromStream<T>(result.Content) ?? throw new DbUnexpectedStateException("FromStream<T> returned null");
                    Debug.Assert(result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Created);
                    Debug.Assert(item.id == newItem.id);
                    Debug.Assert(item.pk == newItem.pk);
                    Debug.Assert(item.Type == type);
                    Debug.Assert(item._etag != newItem._etag);
                    return new ReadOrCreateResult<T>(
                        document: newItem,
                        alreadyExisted: false);
                }
                else if (result.StatusCode != HttpStatusCode.Conflict)
                {
                    throw result.ExceptionFromErrorStatus();
                }
            }

            tryCreateFirst = true;

            var readItem = await ReadByIdAsync<T>(
                id: item.id,
                partitionKey: partitionKey,
                type: type).ConfigureAwait(false);
            if (readItem != null)
            {
                return new ReadOrCreateResult<T>(
                    document: readItem,
                    alreadyExisted: true);
            }
        }

        Debug.Assert(false);
        throw new DbOverloadedException(); // Should never get here
    }

    internal protected virtual async Task<ReplaceResult<T>> ReplaceItemAsync<T>(T item, string type, PartitionKey partitionKey, string partitionKeyString) where T : DbDoc
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        if (partitionKeyString is null)
        {
            throw new ArgumentNullException(nameof(partitionKeyString));
        }

        ThrowIfReadOnly();

        if (item.id is null)
        {
            throw new InvalidOperationException("The document .id property is missing");
        }
        if (item._etag is null)
        {
            throw new InvalidOperationException("The document is missing an etag");
        }
        if (item.pk != partitionKeyString)
        {
            throw new InvalidOperationException("The document .pk property does not match this partition key");
        }
        if (item.Type != type)
        {
            throw new InvalidOperationException($"The document .Type property does not match what was expected ({type})");
        }

        if (ValidateStateBeforeSave)
        {
            item.ValidateStateOrThrow();
        }

        using var payload = Serializer.ToStream(item);
        using var result = await Container.ReplaceItemStreamAsync(
            streamPayload: payload,
            id: item.id,
            partitionKey: partitionKey,
            requestOptions: new ItemRequestOptions { IfMatchEtag = item._etag }).ConfigureAwait(false);
        if (result.IsSuccessStatusCode)
        {
            var newItem = Serializer.FromStream<T>(result.Content) ?? throw new DbUnexpectedStateException("FromStream<T> returned null");
            Debug.Assert(item.id == newItem.id);
            Debug.Assert(item.pk == newItem.pk);
            Debug.Assert(item.Type == type);
            Debug.Assert(item._etag != newItem._etag);
            return new ReplaceResult<T>(newItem);
        }
        return result.ReplaceResultFromErrorStatus<T>();
    }

    internal protected virtual async Task<DbConflictType?> DeleteItemAsync<T>(T item, PartitionKey partitionKey, string partitionKeyString) where T : DbDoc
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        if (partitionKeyString is null)
        {
            throw new ArgumentNullException(nameof(partitionKeyString));
        }

        ThrowIfReadOnly();

        if (item.id is null)
        {
            throw new InvalidOperationException("The document .id property is missing");
        }
        if (item._etag is null)
        {
            throw new InvalidOperationException("The document is missing an etag");
        }
        if (item.pk != partitionKeyString)
        {
            throw new InvalidOperationException("The document .pk property does not match this partition key");
        }

        using var result = await Container.DeleteItemStreamAsync(
            id: item.id,
            partitionKey: partitionKey,
            requestOptions: new ItemRequestOptions { IfMatchEtag = item._etag }).ConfigureAwait(false);
        if (result.IsSuccessStatusCode)
        {
            return null;
        }

        return result.DeleteConflictTypeFromErrorStatus();
    }

    /// <summary>
    /// Executes a query asynchronously.
    /// <see cref="https://github.com/Azure/azure-cosmos-dotnet-v3/blob/bb72ba5786d99d928b4774e16810f2655029e8a2/Microsoft.Azure.Cosmos/src/Linq/CosmosLinqExtensions.cs"/>
    /// </summary>
    public virtual async IAsyncEnumerable<T> ExecuteQueryAsync<T>(
        IQueryable<T> query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        cancellationToken.ThrowIfCancellationRequested();
        var streamIterator = query.ToStreamIterator();
        while (streamIterator.HasMoreResults)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var results = await streamIterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
            if (results.IsSuccessStatusCode)
            {
                foreach (var item in Serializer.DeserializeDocumentList<T>(results.Content))
                {
                    yield return item;
                }
            }
            else
            {
                throw results.ExceptionFromErrorStatus();
            }
        }
    }

#if DEBUG
    /// <summary>
    /// This deletes an item regardless of whether it's listed as "Transient"
    /// Use with care.
    /// This method only exists in DEBUG mode and is intended only for debugging purposes.
    /// </summary>
#pragma warning disable IDE1006 // Naming Styles
    public async Task<DbConflictType?> _DEBUGONLY_DANGEROUS_DIE_DIE_DIE_DeleteItemAsync<T>(T item) where T : DbDoc
#pragma warning restore IDE1006 // Naming Styles
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ThrowIfReadOnly();

        if (item.id is null)
        {
            throw new InvalidOperationException("The document .id property is missing");
        }
        if (item._etag is null)
        {
            throw new InvalidOperationException("The document is missing an etag");
        }

        using var result = await Container.DeleteItemStreamAsync(
            id: item.id,
            partitionKey: new PartitionKey(item.pk),
            requestOptions: new ItemRequestOptions { IfMatchEtag = item._etag }).ConfigureAwait(false);
        if (result.IsSuccessStatusCode)
        {
            return null;
        }

        return result.DeleteConflictTypeFromErrorStatus();
    }
#endif
}
