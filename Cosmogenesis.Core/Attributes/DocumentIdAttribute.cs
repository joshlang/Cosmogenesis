namespace Cosmogenesis.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
/// <summary>
/// Attach this to the method which generates the .id property for a document.
/// By convention, if none exist, a method named GetId will used.
/// It must be static, accessible, and return a string.
/// The parameters used must also exist as properties in the document.
/// </summary>
public sealed class DocumentIdAttribute : Attribute
{
}
