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
    public static void Write(SourceProductionContext context, (Compilation Left, ImmutableArray<SyntaxNode> Right) action)
    {
        if (OutputModelBuilder.Create(context, action.Left, action.Right) is not OutputModel model) { return; }
        ModelValidation.Validate(model);
        if (!model.CanGenerate) { return; }
        var plan = OutputPlanBuilder.Create(model);
        OutputPlanWriter.Write(model, plan);
    }
}
