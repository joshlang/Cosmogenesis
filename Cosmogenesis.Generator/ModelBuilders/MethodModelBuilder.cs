using Cosmogenesis.Generator.Models;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.ModelBuilders;
static class MethodModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, IMethodSymbol symbol)
    {
        outputModel.CancellationToken.ThrowIfCancellationRequested();

        var model = new MethodModel
        {
            MethodSymbol = symbol
        };

        classModel.Methods.Add(model);
    }
}
