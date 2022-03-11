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
    /// <summary>Mocking constructor</summary>
    protected {databasePlan.ChangeFeedProcessorClassName}() {{ }}

    public {databasePlan.ChangeFeedProcessorClassName}(
        Microsoft.Azure.Cosmos.Container databaseContainer,
        Microsoft.Azure.Cosmos.Container leaseContainer,
        string processorName,
        {databasePlan.Namespace}.{databasePlan.BatchHandlersClassName} {databasePlan.BatchHandlersArgumentName},
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
            batchProcessor: new({databasePlan.BatchHandlersArgumentName}))
    {{
    }}
}}
";

        outputModel.Context.AddSource($"feed_{databasePlan.ChangeFeedProcessorClassName}.cs", s);
    }
}
