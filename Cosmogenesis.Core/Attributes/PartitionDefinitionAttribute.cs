namespace Cosmogenesis.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
/// <summary>
/// When attached to a static class, all eligible methods become definitions of a partition with a name matching the method name.
/// Eligible methods are accessible and return a string.
/// </summary>
public sealed class PartitionDefinitionAttribute : Attribute
{
}
