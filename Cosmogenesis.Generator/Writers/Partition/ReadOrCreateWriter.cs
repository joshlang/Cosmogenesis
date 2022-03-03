using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers.Partition;
static class ReadOrCreateWriter
{
    public static void Write(OutputModel outputModel, DatabasePlan databasePlan, PartitionPlan partitionPlan)
    {
        var s = $@"
namespace {databasePlan.Namespace};

public class {partitionPlan.ReadOrCreateClassName}
{{
    protected virtual {databasePlan.Namespace}.{partitionPlan.ClassName} {partitionPlan.ClassName} {{ get; }} = default!;

    /// <summary>Mocking constructor</summary>
    protected {partitionPlan.ReadOrCreateClassName}() {{ }}

    internal protected {partitionPlan.ReadOrCreateClassName}({databasePlan.Namespace}.{partitionPlan.ClassName} {partitionPlan.ClassNameArgument})
    {{
        this.{partitionPlan.ClassName} = {partitionPlan.ClassNameArgument} ?? throw new System.ArgumentNullException(nameof({partitionPlan.ClassNameArgument}));
    }}

{string.Concat(partitionPlan.DocumentsByDocType.Values.Select(x => ReadOrCreate(partitionPlan, x)))}
}}
";

        outputModel.Context.AddSource($"partition_{partitionPlan.ReadOrCreateClassName}.cs", s);
    }

    static string ReadOrCreate(PartitionPlan partitionPlan, DocumentPlan documentPlan) => $@"
    /// <summary>
    /// Read a {documentPlan.ClassName} document, or create it if it does not yet exist.
    /// </summary>
    /// <exception cref=""DbOverloadedException"" />
    /// <exception cref=""DbUnknownStatusCodeException"" />
    public virtual System.Threading.Tasks.Task<Cosmogenesis.Core.ReadOrCreateResult<{documentPlan.FullTypeName}>> {documentPlan.ClassName}Async({$"bool tryCreateFirst, {documentPlan.PropertiesByName.Values.AsInputParameters()}"}) => 
        this.{partitionPlan.ClassName}.ReadOrCreateAsync({documentPlan.ClassNameArgument}: new {documentPlan.FullTypeName} {{ {documentPlan.PropertiesByName.Values.AsSettersFromParameters()} }}, tryCreateFirst: tryCreateFirst);
";
}
