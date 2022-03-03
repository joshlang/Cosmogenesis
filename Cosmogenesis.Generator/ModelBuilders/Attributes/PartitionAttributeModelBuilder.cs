using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Models.Attributes;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.ModelBuilders.Attributes;
static class PartitionAttributeModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, AttributeData attributeData)
    {
        var model = new PartitionAttributeModel();
        if (!attributeData.ConstructorArguments.IsDefaultOrEmpty)
        {
            model.Name = attributeData.ConstructorArguments[0].Value as string ?? "";
        }

        classModel.PartitionAttribute = model;
    }
}
