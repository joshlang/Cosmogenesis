using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator
{
    static class SymbolExtensions
    {
        static readonly SymbolDisplayFormat DisplayFormat = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
        public static string FullTypeName(this ITypeSymbol typeSymbol) => typeSymbol.ToDisplayString(DisplayFormat);

        public static string FullMethodName(this IMethodSymbol methodSymbol)
        {
            List<string> parts = new();
            parts.Add(FullTypeName(methodSymbol.ContainingType));
            parts.Add(methodSymbol.Name);
            return string.Join(".", parts);
        }

        public static string AsInputParameters(this IMethodSymbol methodSymbol) => string.Join(", ",
            methodSymbol
            .Parameters
            .Select(x => $"{x.Type.FullTypeName()} {x.Name}"));

        public static string AsInputParametersForTuple(this IMethodSymbol methodSymbol) => string.Join(", ",
            methodSymbol
            .Parameters
            .Select(x => $"{x.Type.FullTypeName()} {x.Name.CSharpify()}"));

        public static string AsInputParameterMapping(this IMethodSymbol methodSymbol) => string.Join(", ",
            methodSymbol
            .Parameters
            .Select(x => $"{x.Name}: {x.Name}"));

        public static Func<string, string> AsDocumentToParametersMapping(this IMethodSymbol methodSymbol) =>
            (string name) => string.Join(", ", methodSymbol.Parameters.Select(x => $"{x.Name}: {name}.{x.Name.CSharpify()}"));
    }
}
