using Cosmogenesis.Generator.Models;

namespace Cosmogenesis.Generator.Plans;
class OutputPlan
{
    public readonly Dictionary<string, DatabasePlan> DatabasePlansByName = new();
    public readonly Dictionary<ClassModel, List<DatabasePlan>> DatabasePlansByClass = new();
}
