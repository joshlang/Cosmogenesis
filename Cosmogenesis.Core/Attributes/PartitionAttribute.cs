using System;

namespace Cosmogenesis.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    /// <summary>
    /// Specifies in which partition a document belongs.
    /// </summary>
    public sealed class PartitionAttribute : Attribute
    {
        public readonly string? Name;

        public PartitionAttribute(string name)
        {
            Name = name;
        }
    }
}
