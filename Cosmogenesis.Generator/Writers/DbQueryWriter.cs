﻿using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers;
static class DbQueryWriter
{
    public static void Write(OutputModel outputModel, DatabasePlan databasePlan)
    {
        var s = $@"
namespace {databasePlan.Namespace};

public class {databasePlan.QueryClassName} : Cosmogenesis.Core.DbQueryBase
{{
    protected virtual {databasePlan.Namespace}.{databasePlan.DbClassName} {databasePlan.DbClassName} {{ get; }} = default!;
    protected virtual {databasePlan.Namespace}.{databasePlan.QueryBuilderClassName} {databasePlan.QueryBuilderClassName} {{ get; }} = default!;

    /// <summary>Mocking constructor</summary>
    protected {databasePlan.QueryClassName}() {{ }}

    internal protected {databasePlan.QueryClassName}(
        {databasePlan.Namespace}.{databasePlan.DbClassName} {databasePlan.DbClassNameArgument},
        {databasePlan.Namespace}.{databasePlan.QueryBuilderClassName} {databasePlan.QueryBuilderClassNameArgument})
        : base(
            dbBase: {databasePlan.DbClassNameArgument},
            queryBuilder: {databasePlan.QueryBuilderClassNameArgument})
    {{
        this.{databasePlan.DbClassName} = {databasePlan.DbClassNameArgument} ?? throw new System.ArgumentNullException(nameof({databasePlan.DbClassNameArgument}));
        this.{databasePlan.QueryBuilderClassName} = {databasePlan.QueryBuilderClassNameArgument} ?? throw new System.ArgumentNullException(nameof({databasePlan.QueryBuilderClassNameArgument}));
    }}

{string.Concat(databasePlan.PartitionPlansByName.Values.SelectMany(x => x.Documents).Select(x => Query(databasePlan, x)))}
}}
";

        outputModel.Context.AddSource($"db_{databasePlan.QueryClassName}.cs", s);
    }

    static string Query(DatabasePlan databasePlan, DocumentPlan documentPlan) => $@"
    /// <summary>
    /// Build and execute a query filtered to {documentPlan.ClassName} documents.
    /// <see cref=""https://github.com/Azure/azure-cosmos-dotnet-v3/blob/bb72ba5786d99d928b4774e16810f2655029e8a2/Microsoft.Azure.Cosmos/src/Linq/CosmosLinqExtensions.cs"" />
    /// </summary>
    public virtual System.Collections.Generic.IAsyncEnumerable<T> {documentPlan.PluralName}<T>(
        System.Func<System.Linq.IQueryable<{documentPlan.FullTypeName}>, 
        System.Linq.IQueryable<T>> createQuery,
        System.Threading.CancellationToken cancellationToken = default) 
        => this.{databasePlan.DbClassName}
            .ExecuteQueryAsync(
                query: createQuery(this.{databasePlan.QueryBuilderClassName}.{documentPlan.PluralName}()),
                cancellationToken: cancellationToken);

    /// <summary>
    /// Execute a query filtered to {documentPlan.ClassName} documents.
    /// </summary>
    public virtual System.Collections.Generic.IAsyncEnumerable<{documentPlan.FullTypeName}> {documentPlan.PluralName}(
        System.Threading.CancellationToken cancellationToken = default)
        => this.{databasePlan.DbClassName}
            .ExecuteQueryAsync(
                query: this.{databasePlan.QueryBuilderClassName}.{documentPlan.PluralName}(),
                cancellationToken: cancellationToken);
";
}
