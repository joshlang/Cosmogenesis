using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class ChangeFeedHandlersWriter
    {
        public static void Write(GeneratorExecutionContext context, DbModel dbModel)
        {
            var s = $@"
using System;
using System.Threading;
using System.Threading.Tasks;
using Cosmogenesis.Core;

namespace {dbModel.Namespace}
{{
    public class {dbModel.ChangeFeedHandlersClassName} : ChangeFeedHandlersBase
    {{
{string.Concat(dbModel.Partitions.Values.Select(Partition))}

        public virtual void ThrowIfAnyDocumentHandlerNotSet()
        {{
{string.Concat(dbModel.Partitions.Values.SelectMany(x => x.Documents.Values).Select(ThrowIfNotSet))}
        }}
    }}
}}
";

            context.AddSource($"feed_{dbModel.ChangeFeedHandlersClassName}.cs", s);
        }

        static string Partition(DbPartitionModel partitionModel) => $@"
        public class {partitionModel.ChangeFeedHandlersClassName}
        {{
{string.Concat(partitionModel.Documents.Values.Select(HasBeenSet))}
{string.Concat(partitionModel.Documents.Values.Select(Handler))}
        }}
    
        public virtual {partitionModel.ChangeFeedHandlersClassName} {partitionModel.Name} {{ get; }} = new();
";

        static string HasBeenSet(DbDocumentModel documentModel) => $@"
            internal bool Set_{documentModel.ClassName};";

        static string Handler(DbDocumentModel documentModel) => $@"
            Func<{documentModel.ClassFullName}, CancellationToken, Task>? {documentModel.ClassName.Parameterify()};
            public virtual Func<{documentModel.ClassFullName}, CancellationToken, Task>? {documentModel.ClassName}
            {{
                get => {documentModel.ClassName.Parameterify()};
                set
                {{
                    Set_{documentModel.ClassName} = true;
                    {documentModel.ClassName.Parameterify()} = value;
                }}
            }}
";

        static string ThrowIfNotSet(DbDocumentModel documentModel) => $@"
            if (!{documentModel.DbPartitionModel.Name}.Set_{documentModel.ClassName})
            {{
                throw new InvalidOperationException($""Change feed document handler for {documentModel.ClassFullName} was not set."");
            }}";
    }
}
