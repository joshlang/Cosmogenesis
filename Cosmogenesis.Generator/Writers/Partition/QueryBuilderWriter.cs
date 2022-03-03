using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers.Partition;
static class QueryBuilderWriter
{
    public static void Write(OutputModel outputModel, DatabasePlan databasePlan, PartitionPlan partitionPlan)
    {
        var s = $@"
using System.Linq;
using Cosmogenesis.Core;
using Microsoft.Azure.Cosmos;

namespace {databasePlan.Namespace};

public class {partitionPlan.QueryBuilderClassName} : Cosmogenesis.Core.DbQueryBuilderBase
{{
    /// <summary>Mocking constructor</summary>
    protected {partitionPlan.QueryBuilderClassName}() {{ }}

    internal protected {partitionPlan.QueryBuilderClassName}(
        {databasePlan.Namespace}.{databasePlan.DbClassName} {databasePlan.DbClassNameArgument},
        Microsoft.Azure.Cosmos.PartitionKey? partitionKey)
        : base(
            dbBase: {databasePlan.DbClassNameArgument},
            partitionKey: partitionKey)
    {{
    }}

{string.Concat(partitionPlan.DocumentsByDocType.Values.Select(BuildQuery))}
}}
";

        outputModel.Context.AddSource($"partition_{partitionPlan.QueryBuilderClassName}.cs", s);
    }

    static string BuildQuery(DocumentPlan documentPlan) => $@"
    /// <summary>
    /// Build a query filtered to {documentPlan.ClassName} documents.
    /// Additional Linq transformations can be appended.
    /// Use ExecuteQueryAsync to execute.
    /// <see cref=""https://github.com/Azure/azure-cosmos-dotnet-v3/blob/bb72ba5786d99d928b4774e16810f2655029e8a2/Microsoft.Azure.Cosmos/src/Linq/CosmosLinqExtensions.cs"" />
    /// </summary>
    public virtual System.Linq.IQueryable<{documentPlan.FullTypeName}> {documentPlan.ClassName.Pluralize()}() => 
        this.BuildQueryByType<{documentPlan.FullTypeName}>(type: {documentPlan.ConstDocType});
";
}
