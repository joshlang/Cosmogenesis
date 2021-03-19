using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cosmogenesis.Generator
{
    class CosmosSyntaxReceiver : ISyntaxContextReceiver
    {
        public List<AttributeSyntax> DbAttributes = new();
        public List<ClassDeclarationSyntax> SubClasses = new();
        public List<ClassDeclarationSyntax> PartitionDefinitionClasses = new();
        public List<MethodDeclarationSyntax> PartitionDefinitionMethods = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            switch (context.Node)
            {
                case AttributeSyntax attributeSyntax:
                    HandleAttribute(context, attributeSyntax);
                    break;
                case ClassDeclarationSyntax classDeclarationSyntax:
                    HandleClassDeclaration(context, classDeclarationSyntax);
                    break;
                case MethodDeclarationSyntax methodDeclarationSyntax:
                    HandleMethodDeclaration(context, methodDeclarationSyntax);
                    break;
            }
        }
        
        static string? GetAttributeTypeName(GeneratorSyntaxContext context, AttributeSyntax attributeSyntax) =>
            context.SemanticModel.GetTypeInfo(attributeSyntax).Type?.Name;

        void HandleMethodDeclaration(GeneratorSyntaxContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (methodDeclarationSyntax.AttributeLists.Any(x => x.Attributes.Any(a => GetAttributeTypeName(context, a) == Types.PartitionDefinitionAttribute)))
            {
                PartitionDefinitionMethods.Add(methodDeclarationSyntax);
            }
        }

        void HandleAttribute(GeneratorSyntaxContext context, AttributeSyntax attributeSyntax)
        {
            switch (GetAttributeTypeName(context, attributeSyntax))
            {
                case Types.DbAttribute:
                    DbAttributes.Add(attributeSyntax);
                    break;
            }
        }

        void HandleClassDeclaration(GeneratorSyntaxContext context, ClassDeclarationSyntax classDeclarationSyntax)
        {
            if (classDeclarationSyntax.BaseList?.Types.Count > 0)
            {
                SubClasses.Add(classDeclarationSyntax);
            }       
            if (classDeclarationSyntax.AttributeLists.Any(x => x.Attributes.Any(a => GetAttributeTypeName(context, a) == Types.PartitionDefinitionAttribute)))
            {
                PartitionDefinitionClasses.Add(classDeclarationSyntax);
            }
        }
    }
}
