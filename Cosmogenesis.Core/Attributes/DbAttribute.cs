namespace Cosmogenesis.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
/// <summary>
/// Specifies in which database a document exists.
/// </summary>
public sealed class DbAttribute : Attribute
{
    public readonly string Name;
    public string? Namespace { get; set; }

    public DbAttribute(string name)
    {
        Name = name;
    }
}
