﻿using Microsoft.Azure.Cosmos;

namespace Cosmogenesis.Core;

public abstract class DbQueryBuilderBase
{
    protected virtual DbBase DbBase { get; } = default!;
    protected virtual PartitionKey? PartitionKey { get; } = default!;

    protected DbQueryBuilderBase() { }

    protected DbQueryBuilderBase(DbBase dbBase, PartitionKey? partitionKey)
    {
        DbBase = dbBase ?? throw new ArgumentNullException(nameof(dbBase));
        PartitionKey = partitionKey;
    }

    protected virtual IQueryable<T> BuildQueryByType<T>(string type) where T : DbDoc
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return DbBase
            .Container
            .GetItemLinqQueryable<T>(
                allowSynchronousQueryExecution: false,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = PartitionKey
                })
            .Where(x => x.Type == type);
    }

    /// <summary>
    /// Build a query covering all documents.
    /// Linq transformations can be appended.
    /// Use ExecuteQueryAsync to execute.
    /// Example 1: `x => x["ActualFieldName"] == 4`
    /// Example 2: `x => ((SomeDbDoc)(object)x).ActualFieldName == 4`
    /// <see cref=""https://github.com/Azure/azure-cosmos-dotnet-v3/blob/bb72ba5786d99d928b4774e16810f2655029e8a2/Microsoft.Azure.Cosmos/src/Linq/CosmosLinqExtensions.cs"" />
    /// </summary>
    public virtual IQueryable<IDictionary<string, object>> Dynamic() => DbBase
        .Container
        .GetItemLinqQueryable<IDictionary<string, object>>(
            allowSynchronousQueryExecution: false,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = PartitionKey
            });
}
