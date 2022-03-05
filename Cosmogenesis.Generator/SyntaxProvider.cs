using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cosmogenesis.Generator;
static class SyntaxProvider
{
    public static bool Filter(SyntaxNode node, CancellationToken cancellationToken) => node is ClassDeclarationSyntax || node is CompilationUnitSyntax;
    public static SyntaxNode Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken) => context.Node;
}
