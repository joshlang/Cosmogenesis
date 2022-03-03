using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers.Partition;
static class PlanExtensions
{
    public static string DocumentToParametersMapping(this GetPkIdPlan plan, string parameterName) =>
        string.Join(", ", plan.Arguments.Select(x => $"{x.ArgumentName}: {parameterName}.{x.PropertyName}"));

    public static string AsInputParameters(this GetPkIdPlan plan) =>
        string.Join(", ", plan.Arguments.Select(x => $"{x.FullTypeName} {x.ArgumentName}"));

    public static string AsInputParameters(this IEnumerable<PropertyPlan> properties) =>
        string.Join(", ", properties.OrderBy(x => x.UseDefault).Select(x => $"{x.FullTypeName} {x.ArgumentName}{(x.UseDefault ? " = default" : "")}"));

    public static string AsInputParameterMapping(this IEnumerable<PropertyPlan> properties) =>
        string.Join(", ", properties.Select(x => $"{x.ArgumentName}: {x.ArgumentName}"));

    public static string AsInputParameterMapping(this GetPkIdPlan plan) =>
        string.Join(", ", plan.Arguments.Select(x => $"{x.ArgumentName}: {x.ArgumentName}"));

    public static string AsSettersFromParameters(this IEnumerable<PropertyPlan> properties) =>
        string.Join(", ", properties.Select(x => $"{x.PropertyName} = {x.ArgumentName}"));

    public static string ParametersToParametersMapping(this GetPkIdPlan plan, string parameterName) =>
        string.Join(", ", plan.Arguments.Select(x => $"{x.ArgumentName}: {parameterName}.{x.ArgumentName}"));
}
