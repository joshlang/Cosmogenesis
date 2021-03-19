using System;

namespace Cosmogenesis.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    /// <summary>
    /// Attach this to the method which generates the .pk property for a document (The partition key).
    /// It must be static, accessible, and return a string.
    /// The parameters used must also exist as properties in the document.
    /// If a name is not specified, the method name becomes the partition name.
    /// This can be attached to a static class, in which case all eligible methods in the class define partitions.
    /// </summary>
    public sealed class PartitionDefinitionAttribute : Attribute
    {
        public readonly string? Name;

        public PartitionDefinitionAttribute()
        {
        }
        public PartitionDefinitionAttribute(string name)
        {
            Name = name;
        }
    }
}
