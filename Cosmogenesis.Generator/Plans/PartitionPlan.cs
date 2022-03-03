using Cosmogenesis.Generator.Models;

namespace Cosmogenesis.Generator.Plans;
class PartitionPlan
{
    public string Name = default!;
    public string ClassName = default!;
    public string ClassNameArgument = default!;
    public string PluralName = default!;
    public MethodModel GetPkModel = default!;
    public GetPkIdPlan GetPkPlan = default!;
    public readonly Dictionary<string, DocumentPlan> DocumentsByDocType = new();
    public string ChangeFeedHandlersClassName = default!;
    public string CreateOrReplaceClassName = default!;
    public string ReadClassName = default!;
    public string ReadOrThrowClassName = default!;
    public string ReadManyClassName = default!;
    public string QueryBuilderClassName = default!;
    public string QueryClassName = default!;
    public string QueryBuilderClassNameArgument = default!;
    public string ReadOrCreateClassName = default!;
    public string CreateClassName = default!;
    public string BatchClassName = default!;
}
