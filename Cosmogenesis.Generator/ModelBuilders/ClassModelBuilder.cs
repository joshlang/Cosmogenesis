using Cosmogenesis.Generator.ModelBuilders.Attributes;
using Cosmogenesis.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Cosmogenesis.Generator.ModelBuilders;
static class ClassModelBuilder
{
    public static void Build(OutputModel outputModel, ClassDeclarationSyntax syntax)
    {
        outputModel.CancellationToken.ThrowIfCancellationRequested();
        
        var semanticModel = outputModel.Compilation.GetSemanticModel(syntax.SyntaxTree);
        if (semanticModel.GetDeclaredSymbol(syntax) is not INamedTypeSymbol classSymbol) { return; }
        
        var model = new ClassModel
        {
            ClassSymbol = classSymbol
        };

        Build(outputModel, model, classSymbol);

        outputModel.Classes.Add(model);
    }

    static void Build(OutputModel outputModel, ClassModel model, INamedTypeSymbol classSymbol)
    {
        if (classSymbol.BaseType is null) { return; } // Skip System.Object
        if (SymbolEqualityComparer.Default.Equals(outputModel.DbDocSymbol, classSymbol))
        {
            model.IsDbDoc = true;
            return;
        }

        Build(outputModel, model, classSymbol.BaseType);

        foreach (var attributeData in classSymbol.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.DbAttributeSymbol))
            {
                DbAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
            else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.DocTypeAttributeSymbol))
            {
                DocTypeAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
            else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.MutableAttributeSymbol))
            {
                MutableAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
            else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.TransientAttributeSymbol))
            {
                TransientAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
            else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.PartitionAttributeSymbol))
            {
                PartitionAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
            else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, outputModel.PartitionDefinitionAttributeSymbol))
            {
                PartitionDefinitionAttributeModelBuilder.Build(outputModel, model, attributeData);
            }
        }

        foreach (var member in classSymbol.GetMembers())
        {
            switch (member)
            {
                case IPropertySymbol symbol:
                    PropertyModelBuilder.Build(outputModel, model, symbol);
                    break;
                case IMethodSymbol symbol:
                    MethodModelBuilder.Build(outputModel, model, symbol);
                    break;
            }
        }
    }
}
