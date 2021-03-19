using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator
{
    [Generator]
    class CosmosGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) => context.RegisterForSyntaxNotifications(() => new CosmosSyntaxReceiver());

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not CosmosSyntaxReceiver syntaxReceiver)
            {
                return;
            }

            var executor = new Executor(context, syntaxReceiver);
            executor.Generate();
        }
    }
}
