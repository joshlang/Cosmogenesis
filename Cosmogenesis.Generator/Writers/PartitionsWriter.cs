using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;
using Cosmogenesis.Generator.Writers.Partition;

namespace Cosmogenesis.Generator.Writers;
static class PartitionsWriter
{
    public static void Write(OutputModel outputModel, DatabasePlan databasePlan)
    {
        var s = $@"
namespace {databasePlan.Namespace};

public class {databasePlan.PartitionsClassName}
{{
    protected virtual {databasePlan.DbClassName} {databasePlan.DbClassName} {{ get; }} = default!;

    /// <summary>Mocking constructor</summary>
    protected {databasePlan.PartitionsClassName}() {{ }}

    internal protected {databasePlan.PartitionsClassName}({databasePlan.DbClassName} {databasePlan.DbClassNameArgument})
    {{
        this.{databasePlan.DbClassName} = {databasePlan.DbClassNameArgument} ?? throw new System.ArgumentNullException(nameof({databasePlan.DbClassNameArgument}));
    }}

{string.Concat(databasePlan.PartitionPlansByName.Values.Select(x => Partition(databasePlan, x)))}
}}
";
        outputModel.Context.AddSource($"db_{databasePlan.PartitionsClassName}.cs", s);

        foreach (var partition in databasePlan.PartitionPlansByName.Values)
        {
            PartitionWriter.Write(outputModel, databasePlan, partition);
        }
    }

    static string Partition(DatabasePlan databasePlan, PartitionPlan partitionPlan) => $@"
    public virtual {databasePlan.Namespace}.{partitionPlan.ClassName} {partitionPlan.Name}({partitionPlan.GetPkPlan.AsInputParameters()}) =>
        new(
            {databasePlan.DbClassNameArgument}: this.{databasePlan.DbClassName},
            partitionKey: {partitionPlan.GetPkPlan.FullMethodName}({partitionPlan.GetPkPlan.AsInputParameterMapping()}));
";
}
