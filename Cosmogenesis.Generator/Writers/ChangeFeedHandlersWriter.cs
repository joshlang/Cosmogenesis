using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers;
static class ChangeFeedHandlersWriter
{
    public static void Write(OutputModel outputModel, DatabasePlan databasePlan)
    {
        var s = $@"
namespace {databasePlan.Namespace};

public class {databasePlan.ChangeFeedHandlersClassName} : Cosmogenesis.Core.ChangeFeedHandlersBase
{{
{string.Concat(databasePlan.PartitionPlansByName.Values.Select(Partition))}

    public virtual void ThrowIfAnyDocumentHandlerNotSet()
    {{
{string.Concat(databasePlan.PartitionPlansByName.Values.SelectMany(x => x.Documents.Select(d => ThrowIfNotSet(x, d))))}
    }}
}}
";

        outputModel.Context.AddSource($"feed_{databasePlan.ChangeFeedHandlersClassName}.cs", s);
    }

    static string Partition(PartitionPlan partitionPlan) => $@"
    public class {partitionPlan.ChangeFeedHandlersClassName}
    {{
{string.Concat(partitionPlan.Documents.Select(HasBeenSet))}
{string.Concat(partitionPlan.Documents.Select(Handler))}
    }}
    
    public virtual {partitionPlan.ChangeFeedHandlersClassName} {partitionPlan.Name} {{ get; }} = new();
";

    static string HasBeenSet(DocumentPlan documentPlan) => $@"
        internal bool Set_{documentPlan.ClassName};";

    static string Handler(DocumentPlan documentPlan) => $@"
        System.Func<{documentPlan.FullTypeName}, System.Threading.CancellationToken, System.Threading.Tasks.Task>? {documentPlan.ClassNameArgument};
        public virtual System.Func<{documentPlan.FullTypeName}, System.Threading.CancellationToken, System.Threading.Tasks.Task>? {documentPlan.ClassName}
        {{
            get => this.{documentPlan.ClassNameArgument};
            set
            {{
                this.Set_{documentPlan.ClassName} = true;
                this.{documentPlan.ClassNameArgument} = value;
            }}
        }}
";

    static string ThrowIfNotSet(PartitionPlan partitionPlan, DocumentPlan documentPlan) => $@"
        if (!this.{partitionPlan.Name}.Set_{documentPlan.ClassName})
        {{
            throw new System.InvalidOperationException($""Change feed document handler for {documentPlan.FullTypeName} was not set."");
        }}";
}
