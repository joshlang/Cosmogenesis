using System.Collections.Immutable;
using Cosmogenesis.Generator.ModelBuilders;
using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.PlanBuilders;
using Cosmogenesis.Generator.Writers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cosmogenesis.Generator;
static class SourceOutput
{
    public static void Write(SourceProductionContext context, (Compilation Left, ImmutableArray<ClassDeclarationSyntax?> Right) action)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        if (action.Right.IsDefaultOrEmpty) { return; }

        var model = new OutputModel
        {
            Context = context,
            CancellationToken = context.CancellationToken,
            Compilation = action.Left,
            DbAttributeSymbol = action.Left.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.DbAttribute")!,
            DocTypeAttributeSymbol = action.Left.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.DocTypeAttribute")!,
            MutableAttributeSymbol = action.Left.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.MutableAttribute")!,
            PartitionAttributeSymbol = action.Left.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.PartitionAttribute")!,
            PartitionDefinitionAttributeSymbol = action.Left.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.PartitionDefinitionAttribute")!,
            TransientAttributeSymbol = action.Left.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.TransientAttribute")!,
            UseDefaultAttributeSymbol = action.Left.GetTypeByMetadataName("Cosmogenesis.Core.Attributes.UseDefaultAttribute")!,
            DbDocSymbol = action.Left.GetTypeByMetadataName("Cosmogenesis.Core.DbDoc")!,
            JsonIgnoreAttributeSymbol = action.Left.GetTypeByMetadataName("System.Text.Json.Serialization.JsonIgnoreAttribute")
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
            return;
        }
        if (action.Left.Options.NullableContextOptions != NullableContextOptions.Enable)
        {
            model.Report(Diagnostics.Errors.NullableContext, action.Left.Assembly);
        }
        foreach (var syntax in action.Right.Where(x => x is not null))
        {
            ClassModelBuilder.Build(model, syntax!);
        }
        ModelValidation.Validate(model);
        if (!model.CanGenerate) { return; }
        var plan = OutputPlanBuilder.Create(model);
        OutputPlanWriter.Write(model, plan);
    }
}
