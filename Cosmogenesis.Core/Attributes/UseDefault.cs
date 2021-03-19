using System;

namespace Cosmogenesis.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    /// <summary>
    /// When a document is created, this property's parameter will have a default value.
    /// </summary>
    public sealed class UseDefaultAttribute : Attribute
    {
    }
}
