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
    /// <summary>Mocking constructor</summary>
    protected {databasePlan.BatchHandlersClassName}() {{ }}

    public {databasePlan.BatchHandlersClassName}({ConstructorArgs(databasePlan)}
        System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task>? newChangeFeedBatch = null,
        System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task>? finishingBatch = null)
    {{
        this.NewChangeFeedBatch = newChangeFeedBatch;
        this.FinishingBatch = finishingBatch;
        {string.Concat(databasePlan.PartitionPlansByName.Values.Select(AssignArg))}
    }}

{string.Concat(databasePlan.PartitionPlansByName.Values.Select(Partition))}

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

    static string ConstructorArgs(DatabasePlan databasePlan) =>
        string.Concat(databasePlan.PartitionPlansByName.Values.Select(ConstructorArg));

    static string ConstructorArg(PartitionPlan partitionPlan) => $@"
        {partitionPlan.BatchHandlersClassName}? {partitionPlan.BatchHandlersClassNameArgument},";

    static string AssignArg(PartitionPlan partitionPlan) => $@"
        this.{partitionPlan.Name} = {partitionPlan.BatchHandlersClassNameArgument};";

    static string Partition(PartitionPlan partitionPlan) => $@"
    public class {partitionPlan.BatchHandlersClassName}
    {{
        /// <summary>Mocking constructor</summary>
        protected {partitionPlan.BatchHandlersClassName}() {{ }}

        public {partitionPlan.BatchHandlersClassName}({partitionPlan.Documents.OrderBy(x => x.ClassName).Select(ConstructorArg).JoinNonEmpty()})
        {{
{string.Concat(partitionPlan.Documents.Select(AssignArg))}
        }}

{string.Concat(partitionPlan.Documents.Select(Handler))}
    }}
    
    public virtual {partitionPlan.BatchHandlersClassName}? {partitionPlan.Name} {{ get; }}
";

    public static string ConstructorArg(DocumentPlan documentPlan) => @$"
            System.Func<{documentPlan.FullTypeName}, System.Threading.CancellationToken, System.Threading.Tasks.Task>? {documentPlan.ClassNameArgument}";

    public static string AssignArg(DocumentPlan documentPlan) => $@"
            this.{documentPlan.ClassName} = {documentPlan.ClassNameArgument};";

    static string Handler(DocumentPlan documentPlan) => $@"
        public virtual System.Func<{documentPlan.FullTypeName}, System.Threading.CancellationToken, System.Threading.Tasks.Task>? {documentPlan.ClassName} {{ get; }}";

    static string CallHandler(DatabasePlan databasePlan, PartitionPlan partitionPlan, DocumentPlan documentPlan) => $@"
            {documentPlan.FullTypeName} x => this.{partitionPlan.Name}?.{documentPlan.ClassName}?.Invoke(x, cancellationToken),";
}
