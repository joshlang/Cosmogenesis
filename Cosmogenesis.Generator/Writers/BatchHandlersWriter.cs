using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers;
static class BatchHandlersWriter
{
    public static void Write(OutputModel outputModel, DatabasePlan databasePlan)
    {
        var s = $@"
namespace {databasePlan.Namespace};

public class {databasePlan.BatchHandlersClassName} : Cosmogenesis.Core.BatchHandlersBase
{{
{string.Concat(databasePlan.PartitionPlansByName.Values.Select(Partition))}

    public virtual void ThrowIfAnyDocumentHandlerNotSet()
    {{
{string.Concat(databasePlan.PartitionPlansByName.Values.SelectMany(x => x.Documents.Select(d => ThrowIfNotSet(x, d))))}
    }}

    public override System.Threading.Tasks.Task? GetHandlerTask(
        Cosmogenesis.Core.DbDoc doc, 
        System.Threading.CancellationToken cancellationToken) => doc switch
        {{
    {string.Concat(databasePlan.PartitionPlansByName.Values.SelectMany(x => x.Documents.Select(d => CallHandler(databasePlan, x, d))))}
            _ => throw new System.NotSupportedException($""Document of type {{doc?.GetType().Name}} was unexpected"")
        }};
}}
";

        outputModel.Context.AddSource($"feed_{databasePlan.BatchHandlersClassName}.cs", s);
    }

    static string Partition(PartitionPlan partitionPlan) => $@"
    public class {partitionPlan.BatchHandlersClassName}
    {{
{string.Concat(partitionPlan.Documents.Select(HasBeenSet))}
{string.Concat(partitionPlan.Documents.Select(Handler))}
    }}
    
    public virtual {partitionPlan.BatchHandlersClassName} {partitionPlan.Name} {{ get; }} = new();
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

    static string CallHandler(DatabasePlan databasePlan, PartitionPlan partitionPlan, DocumentPlan documentPlan) => $@"
            {documentPlan.FullTypeName} x => this.{partitionPlan.Name}.{documentPlan.ClassName}?.Invoke(x, cancellationToken),";
}
