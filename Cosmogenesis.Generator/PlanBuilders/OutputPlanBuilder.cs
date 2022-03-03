using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.PlanBuilders;
static class OutputPlanBuilder
{
    public static OutputPlan Create(OutputModel outputModel)
    {
        outputModel.CancellationToken.ThrowIfCancellationRequested();

        var plan = new OutputPlan();
        DatabasePlanBuilder.Build(outputModel, plan);

        return plan;
    }
}
