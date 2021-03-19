using System;

namespace Cosmogenesis.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    /// <summary>
    /// Specifies a document is transient (can be deleted after creation).
    /// </summary>
    public sealed class TransientAttribute : Attribute
    {
    }
}
