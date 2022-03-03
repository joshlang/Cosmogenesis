﻿using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers.Partition;
static class CreateWriter
{
    public static void Write(OutputModel outputModel, DatabasePlan databasePlan, PartitionPlan partitionPlan)
    {
        var s = $@"
namespace {databasePlan.Namespace};

public class {partitionPlan.CreateClassName}
{{
    protected virtual {databasePlan.Namespace}.{partitionPlan.ClassName} {partitionPlan.ClassName} {{ get; }} = default!;

    /// <summary>Mocking constructor</summary>
    protected {partitionPlan.CreateClassName}() {{ }}

    internal protected {partitionPlan.CreateClassName}({databasePlan.Namespace}.{partitionPlan.ClassName} {partitionPlan.ClassNameArgument})
    {{
        this.{partitionPlan.ClassName} = {partitionPlan.ClassNameArgument} ?? throw new System.ArgumentNullException(nameof({partitionPlan.ClassNameArgument}));
    }}

{string.Concat(partitionPlan.DocumentsByDocType.Values.Select(x => Create(partitionPlan, x)))}
}}
";

        outputModel.Context.AddSource($"partition_{partitionPlan.CreateClassName}.cs", s);
    }

    static string Create(PartitionPlan partitionPlan, DocumentPlan documentPlan) => $@"
    /// <summary>
    /// Try to create a {documentPlan.ClassName}.
    /// </summary>
    /// <exception cref=""DbOverloadedException"" />
    /// <exception cref=""DbUnknownStatusCodeException"" />
    public virtual System.Threading.Tasks.Task<Cosmogenesis.Core.CreateResult<{documentPlan.FullTypeName}>> {documentPlan.ClassName}Async({documentPlan.PropertiesByName.Values.AsInputParameters()}) => 
        this.{partitionPlan.ClassName}.CreateAsync({documentPlan.ClassNameArgument}: new {documentPlan.FullTypeName} {{ {documentPlan.PropertiesByName.Values.AsSettersFromParameters()} }});
";
}
