using Cosmogenesis.Generator.ModelBuilders.Attributes;
using Cosmogenesis.Generator.Models;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.ModelBuilders;
static class PropertyModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, IPropertySymbol symbol)
    {
        outputModel.CancellationToken.ThrowIfCancellationRequested();

        var model = new PropertyModel
        {
            PropertySymbol = symbol
        };

        foreach (var attributeData in symbol.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.UseDefaultAttributeSymbol))
            {
                UseDefaultAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
        }

        classModel.Properties.Add(model);
    }
}
