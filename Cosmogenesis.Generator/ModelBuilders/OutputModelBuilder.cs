using System.Collections.Immutable;
using Cosmogenesis.Generator.ModelBuilders.Attributes;
using Cosmogenesis.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cosmogenesis.Generator.ModelBuilders;
static class OutputModelBuilder
{
    public static OutputModel? Create(SourceProductionContext context, Compilation compilation, ImmutableArray<SyntaxNode> syntaxNodes)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        if (syntaxNodes.IsDefaultOrEmpty) { return null; }

        var model = new OutputModel
        {
            Context = context,
            CancellationToken = context.CancellationToken,
            Compilation = compilation,
            DbAttributeSymbol = compilation.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.DbAttribute")!,
            DocTypeAttributeSymbol = compilation.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.DocTypeAttribute")!,
            MutableAttributeSymbol = compilation.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.MutableAttribute")!,
            PartitionAttributeSymbol = compilation.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.PartitionAttribute")!,
            PartitionDefinitionAttributeSymbol = compilation.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.PartitionDefinitionAttribute")!,
            TransientAttributeSymbol = compilation.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.TransientAttribute")!,
            UseDefaultAttributeSymbol = compilation.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.UseDefaultAttribute")!,
            DbDocSymbol = compilation.GetTypeByMetadataName("Cosmogenesis.Core.DbDoc")!,
            JsonIgnoreAttributeSymbol = compilation.GetTypeByMetadataName("System.Text.Json.Serialization.JsonIgnoreAttribute")
        };
        if (model.DbAttributeSymbol is null ||
            model.DocTypeAttributeSymbol is null ||
            model.MutableAttributeSymbol is null ||
            model.PartitionAttributeSymbol is null ||
            model.PartitionDefinitionAttributeSymbol is null ||
            model.TransientAttributeSymbol is null ||
            model.UseDefaultAttributeSymbol is null ||
            model.DbDocSymbol is null)
        {
            return null;
        }
        if (compilation.Options.NullableContextOptions != NullableContextOptions.Enable)
        {
            model.Report(Diagnostics.Errors.NullableContext, compilation.Assembly);
            return null;
        }
        foreach (var attributeData in compilation.Assembly.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, model.DbAttributeSymbol))
            {
                DbAttributeModelBuilder.Build(model, null, attributeData);
            }
        }
        foreach (var syntax in syntaxNodes.Select(x => x as ClassDeclarationSyntax).Where(x => x is not null))
        {
            ClassModelBuilder.Build(model, syntax!);
        }

        return model;
    }
}
