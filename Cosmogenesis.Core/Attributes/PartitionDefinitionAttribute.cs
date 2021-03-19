using System;

namespace Cosmogenesis.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
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
