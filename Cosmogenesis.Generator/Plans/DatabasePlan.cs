using Cosmogenesis.Generator.Models;

namespace Cosmogenesis.Generator.Plans;
class DatabasePlan
{
    public string Namespace = default!;
    public string Name = default!;
    public bool IsDefaultNamespace = true;
    public ClassModel? ClassModel = default!;
    public readonly Dictionary<string, PartitionPlan> PartitionPlansByName = new();
    public string DbClassName = default!;
    public string DbClassNameArgument = default!;
    public string PartitionsClassName = default!;
    public string QueryBuilderClassName = default!;
    public string QueryBuilderClassNameArgument = default!;
    public string QueryClassName = default!;
    public string ReadClassName = default!;
    public string SerializerClassName = default!;
    public string ConverterClassName = default!;
    public string TypesClassName = default!;
    public string ChangeFeedProcessorClassName = default!;
    public string ChangeFeedHandlersClassName = default!;
    public string ChangeFeedHandlersArgumentName = default!;
}
