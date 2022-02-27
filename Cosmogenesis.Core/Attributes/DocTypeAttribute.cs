namespace Cosmogenesis.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
/// <summary>
/// Specifies the .Type field assigned to a document.
/// </summary>
public sealed class DocTypeAttribute : Attribute
{
    public readonly string Name;

    public DocTypeAttribute(string name)
    {
        Name = name;
    }
}
