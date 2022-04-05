using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers;
static class DbQueryBuilderUnionsWriter
{
    public static void Write(OutputModel outputModel, DatabasePlan databasePlan)
    {
        var anyUnions = databasePlan.PartitionPlansByName.Values.SelectMany(x => x.Unions).Any();

        var s = $@"
namespace {databasePlan.Namespace};

public class {databasePlan.QueryBuilderUnionsClassName} : Cosmogenesis.Core.DbQueryBuilderBase
{{
    /// <summary>Mocking constructor</summary>
    protected {databasePlan.QueryBuilderUnionsClassName}() {{ }}

    internal protected {databasePlan.QueryBuilderUnionsClassName}(
        {databasePlan.Namespace}.{databasePlan.DbClassName} {databasePlan.DbClassNameArgument})
        : base(
            dbBase: {databasePlan.DbClassNameArgument},
            partitionKey: null)
    {{
    }}

{string.Concat(databasePlan.PartitionPlansByName.Values.SelectMany(x => x.Unions).Select(BuildQuery))}
}}
";
        
        outputModel.Context.AddSource($"db_{databasePlan.QueryBuilderUnionsClassName}.cs", s);
    }


    static string BuildQuery(UnionPlan unionPlan) => $@"
    static readonly string[] {unionPlan.CommonName}_Types = new[] {{ {string.Join(", ", unionPlan.Documents.Select(x => x.ConstDocType))} }};

    /// <summary>
    /// Build a query filtered to {unionPlan.CommonName} documents.
    /// {unionPlan.CommonName} is a union of: {string.Join(", ", unionPlan.Documents.Select(x => x.ClassName))}
    /// Additional Linq transformations can be appended.
    /// Use ExecuteQueryAsync to execute.
    /// <see cref=""https://github.com/Azure/azure-cosmos-dotnet-v3/blob/bb72ba5786d99d928b4774e16810f2655029e8a2/Microsoft.Azure.Cosmos/src/Linq/CosmosLinqExtensions.cs"" />
    /// </summary>
    public virtual System.Linq.IQueryable<{unionPlan.FullCommonTypeName}> {unionPlan.CommonName.Pluralize()}() => 
        this.BuildQueryByTypes<{unionPlan.FullCommonTypeName}>(types: {unionPlan.CommonName}_Types);
";
}
