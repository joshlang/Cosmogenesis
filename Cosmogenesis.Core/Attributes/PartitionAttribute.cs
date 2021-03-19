using System;

namespace Cosmogenesis.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class PartitionAttribute : Attribute
    {
        public readonly string? Name;

        public PartitionAttribute()
        {
        }
        public PartitionAttribute(string name)
        {
            Name = name;
        }
    }
}
