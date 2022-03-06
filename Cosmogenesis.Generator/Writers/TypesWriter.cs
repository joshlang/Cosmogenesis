using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers;
static class TypesWriter
{
    public static void Write(OutputModel outputModel, DatabasePlan databasePlan)
    {
        var s = $@"
namespace {databasePlan.Namespace};

public static class {databasePlan.TypesClassName}
{{
{string.Concat(databasePlan.PartitionPlansByName.Values.Select(PartitionTypes))}
}}
";

        outputModel.Context.AddSource($"db_{databasePlan.TypesClassName}.cs", s);
    }
    static string PartitionTypes(PartitionPlan partitionPlan) => $@"
    public static class {partitionPlan.ClassName}
    {{
{string.Concat(partitionPlan.DocumentsByDocType.Values.Select(Type))}
    }}
";

    static string Type(DocumentPlan documentPlan) => $@"
        public const string {documentPlan.ClassName} = ""{documentPlan.DocType}"";";
}
