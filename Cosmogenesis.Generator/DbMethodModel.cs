using System;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator
{
    class DbMethodModel
    {
        public readonly IMethodSymbol MethodSymbol;

        public readonly string FullMethodName;
        public readonly string InputParametersForTuple;
        public readonly string InputParameters;
        public readonly string InputParameterMapping;
        public readonly bool HasParameters;
        public readonly Func<string, string> DocumentToParametersMapping;

        public DbMethodModel(IMethodSymbol methodSymbol)
        {
            MethodSymbol = methodSymbol;

            FullMethodName = methodSymbol.FullMethodName();
            InputParameters = methodSymbol.AsInputParameters();
            InputParametersForTuple = methodSymbol.AsInputParametersForTuple();
            InputParameterMapping = methodSymbol.AsInputParameterMapping();
            DocumentToParametersMapping = methodSymbol.AsDocumentToParametersMapping();

            HasParameters = !methodSymbol.Parameters.IsDefaultOrEmpty;
        }
    }
}
