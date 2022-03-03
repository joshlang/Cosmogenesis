using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Models.Attributes;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.ModelBuilders.Attributes;
static class DocTypeAttributeModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, AttributeData attributeData)
    {
        var model = new DocTypeAttributeModel();
        if (!attributeData.ConstructorArguments.IsDefaultOrEmpty)
        {
            model.Name = attributeData.ConstructorArguments[0].Value as string ?? "";
        }

        classModel.DocTypeAttribute = model;
    }
}
