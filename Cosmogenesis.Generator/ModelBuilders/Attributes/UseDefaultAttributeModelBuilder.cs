using Cosmogenesis.Generator.Models;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.ModelBuilders.Attributes;
static class UseDefaultAttributeModelBuilder
{
    public static void Build(OutputModel outputModel, PropertyModel propertyModel, AttributeData attributeData) => propertyModel.UseDefaultAttribute = new();
}
