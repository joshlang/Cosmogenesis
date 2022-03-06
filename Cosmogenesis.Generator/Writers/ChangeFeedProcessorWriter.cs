using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers;
static class ChangeFeedProcessorWriter
{
    public static void Write(OutputModel outputModel, DatabasePlan databasePlan)
    {
        var s = $@"
namespace {databasePlan.Namespace};

public class {databasePlan.ChangeFeedProcessorClassName} : Cosmogenesis.Core.ChangeFeedProcessorBase
{{
    protected virtual {databasePlan.Namespace}.{databasePlan.ChangeFeedHandlersClassName} {databasePlan.ChangeFeedHandlersClassName} {{ get; }} = default!;

    /// <summary>Mocking constructor</summary>
    protected {databasePlan.ChangeFeedProcessorClassName}() {{ }}

    public {databasePlan.ChangeFeedProcessorClassName}(
        Microsoft.Azure.Cosmos.Container databaseContainer,
        Microsoft.Azure.Cosmos.Container leaseContainer,
        string processorName,
        {databasePlan.Namespace}.{databasePlan.ChangeFeedHandlersClassName} {databasePlan.ChangeFeedHandlersArgumentName},
        int maxItemsPerBatch = DefaultMaxItemsPerBatch,
        System.TimeSpan? pollInterval = null,
        System.DateTime? startTime = null) 
        : base (
            processorName: processorName,
            maxItemsPerBatch: maxItemsPerBatch,
            pollInterval: pollInterval,
            startTime: startTime,
            databaseContainer: databaseContainer,
            leaseContainer: leaseContainer,
            changeFeedHandlers: {databasePlan.ChangeFeedHandlersArgumentName})
    {{
        this.{databasePlan.ChangeFeedHandlersClassName} = {databasePlan.ChangeFeedHandlersArgumentName} ?? throw new System.ArgumentNullException(nameof({databasePlan.ChangeFeedHandlersArgumentName}));

        {databasePlan.ChangeFeedHandlersArgumentName}.ThrowIfAnyDocumentHandlerNotSet();
    }}

    protected override System.Threading.Tasks.Task? GetHandlerTask(
        Cosmogenesis.Core.DbDoc doc, 
        System.Threading.CancellationToken cancellationToken) => doc switch
        {{
    {string.Concat(databasePlan.PartitionPlansByName.Values.SelectMany(x => x.Documents.Select(d => CallHandler(databasePlan, x, d))))}
            _ => throw new System.NotSupportedException($""Document of type {{doc?.GetType().Name}} was unexpected"")
        }};
}}
";

        outputModel.Context.AddSource($"feed_{databasePlan.ChangeFeedProcessorClassName}.cs", s);
    }

    static string CallHandler(DatabasePlan databasePlan, PartitionPlan partitionPlan, DocumentPlan documentPlan) => $@"
            {documentPlan.FullTypeName} x => this.{databasePlan.ChangeFeedHandlersClassName}.{partitionPlan.Name}.{documentPlan.ClassName}?.Invoke(x, cancellationToken),";
}
