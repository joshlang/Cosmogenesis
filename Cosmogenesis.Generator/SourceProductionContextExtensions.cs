using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator;
static class SourceProductionContextExtensions
{
    public static void Report(this SourceProductionContext context, DiagnosticDescriptor diagnosticDescriptor, SyntaxNode? syntax, params object?[]? args) =>
        context.ReportDiagnostic(Diagnostic.Create(diagnosticDescriptor, syntax?.GetLocation(), args));

    public static void Report(this SourceProductionContext context, DiagnosticDescriptor diagnosticDescriptor, ISymbol? symbol, params object?[]? args) =>
        context.ReportDiagnostic(Diagnostic.Create(diagnosticDescriptor, symbol?.Locations.FirstOrDefault(), args));
}
