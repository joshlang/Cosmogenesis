namespace Cosmogenesis.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
/// <summary>
/// Specifies a document is mutable (can be changed after creation).
/// </summary>
public sealed class MutableAttribute : Attribute
{
}
