using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cosmogenesis.Generator;
static class SyntaxProvider
{
    public static bool Filter(SyntaxNode node, CancellationToken cancellationToken)
    {
        if (node is not ClassDeclarationSyntax syntax) { return false; }
        cancellationToken.ThrowIfCancellationRequested();
        foreach (var attributeListSyntax in syntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                var name = attributeSyntax.Name.ToString();
                if (name.Contains("Db") ||
                    name.Contains("DocType") ||
                    name.Contains("Mutable") ||
                    name.Contains("Partition") ||
                    name.Contains("Transient"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static ClassDeclarationSyntax? Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken) => context.Node as ClassDeclarationSyntax;
}
