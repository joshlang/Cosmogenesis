using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.PlanBuilders;
static class PropertyPlanBuilder
{
    public static void AddProperties(OutputModel outputModel, ClassModel classModel, DocumentPlan documentPlan)
    {
        outputModel.CancellationToken.ThrowIfCancellationRequested();
        if (!outputModel.CanGenerate) { return; }

        foreach (var propertyModel in classModel.Properties)
        {
            var symbol = propertyModel.PropertySymbol;
            if (outputModel.CanIncludeProperty(symbol))
            {
                var propertyPlan = new PropertyPlan
                {
                    PropertyName = symbol.Name,
                    ArgumentName = symbol.Name.ToArgumentName(),
                    FullTypeName = symbol.Type.ToDisplayString(),
                    UseDefault = propertyModel.UseDefaultAttribute is not null,
                    IsInitOnly = propertyModel.IsInitOnly,
                    PropertyModel = propertyModel
                };
                outputModel.ValidateIdentifiers(
                    symbol,
                    propertyPlan.PropertyName,
                    propertyPlan.ArgumentName);
                documentPlan.PropertiesByName[propertyPlan.PropertyName] = propertyPlan;
                if (documentPlan.PropertiesByArgumentName.ContainsKey(propertyPlan.ArgumentName))
                {
                    outputModel.Report(Diagnostics.Errors.PropertyArgumentCollision, symbol, propertyPlan.ArgumentName);
                }
                else
                {
                    documentPlan.PropertiesByArgumentName[propertyPlan.ArgumentName] = propertyPlan;
                    if (!propertyPlan.IsInitOnly && 
                        !documentPlan.IsMutable &&
                        SymbolEqualityComparer.Default.Equals(propertyPlan.PropertyModel.PropertySymbol.ContainingType, classModel.ClassSymbol))
                    {                        
                        outputModel.Report(Diagnostics.Warnings.InitOnlyNotMutable, symbol, propertyPlan.PropertyName);
                    }
                }
            }
        }
    }
}
