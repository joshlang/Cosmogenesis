using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Models.Attributes;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.ModelBuilders.Attributes;
static class DbAttributeModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, AttributeData attributeData)
    {
        var model = new DbAttributeModel();

        if (!attributeData.ConstructorArguments.IsDefaultOrEmpty)
        {
            model.Name = attributeData.ConstructorArguments[0].Value as string ?? "";
        }

        foreach (var named in attributeData.NamedArguments)
        {
            switch (named.Key)
            {
                case "Namespace":
                    model.Namespace = named.Value.Value as string;
                    break;
            }
        }

        classModel.DbAttributes.Add(model);
    }
}
