using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator;

[Generator(LanguageNames.CSharp)]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var declarations = context
            .SyntaxProvider
            .CreateSyntaxProvider(SyntaxProvider.Filter, SyntaxProvider.Transform)
            .Where(x => x is not null);

        var compilationsAndDeclarations = context.CompilationProvider.Combine(declarations.Collect());

        context.RegisterSourceOutput(compilationsAndDeclarations, SourceOutput.Write);
    }
}
