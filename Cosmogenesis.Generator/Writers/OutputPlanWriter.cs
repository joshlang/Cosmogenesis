using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers;
static class OutputPlanWriter
{
    public static void Write(OutputModel outputModel, OutputPlan outputPlan)
    {
        outputModel.CancellationToken.ThrowIfCancellationRequested();
        if (!outputModel.CanGenerate) { return; }

        foreach (var databasePlan in outputPlan.DatabasePlansByName.Values)
        {
            DatabasePlanWriter.Write(outputModel, databasePlan);
        }
    }
}
