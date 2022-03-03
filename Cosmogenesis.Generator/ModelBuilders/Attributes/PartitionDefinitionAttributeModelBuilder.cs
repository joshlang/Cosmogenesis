using Cosmogenesis.Generator.Models;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.ModelBuilders.Attributes;
static class PartitionDefinitionAttributeModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, AttributeData attributeData) => classModel.PartitionDefinitionAttribute = new();
}
