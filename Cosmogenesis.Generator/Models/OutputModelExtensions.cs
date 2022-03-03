using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Models;
static class OutputModelExtensions
{
    public static void Report(this OutputModel model, DiagnosticDescriptor diagnosticDescriptor, SyntaxNode? syntax, params object?[]? args)
    {
        model.Context.Report(diagnosticDescriptor, syntax, args);
        if (diagnosticDescriptor.DefaultSeverity == DiagnosticSeverity.Error)
        {
            model.CanGenerate = false;
        }
    }

    public static void Report(this OutputModel model, DiagnosticDescriptor diagnosticDescriptor, ISymbol? symbol, params object?[]? args)
    {
        model.Context.Report(diagnosticDescriptor, symbol, args);
        if (diagnosticDescriptor.DefaultSeverity == DiagnosticSeverity.Error)
        {
            model.CanGenerate = false;
        }
    }

    public static bool CanIncludeProperty(this OutputModel model, IPropertySymbol symbol) =>
        !symbol.IsStatic &&
        symbol.GetMethod is not null &&
        symbol.SetMethod is not null &&
        symbol.DeclaredAccessibility.IsAccessible() &&
        symbol.GetMethod.DeclaredAccessibility.IsAccessible() &&
        symbol.SetMethod.DeclaredAccessibility.IsAccessible() &&
        (
            model.JsonIgnoreAttributeSymbol is null ||
            !symbol.GetAttributes().Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, model.JsonIgnoreAttributeSymbol))
        );
}
