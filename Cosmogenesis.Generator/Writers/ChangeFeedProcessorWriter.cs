using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class ChangeFeedProcessorWriter
    {
        public static void Write(GeneratorExecutionContext context, DbModel dbModel)
        {
            var s = $@"
using System;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using System.Threading;
using Cosmogenesis.Core;

namespace {dbModel.Namespace}
{{
    public class {dbModel.ChangeFeedProcessorClassName} : ChangeFeedProcessorBase
    {{
        protected virtual {dbModel.ChangeFeedHandlersClassName} {dbModel.ChangeFeedHandlersClassName} {{ get; }} = default!;

        /// <summary>Mocking constructor</summary>
        protected {dbModel.ChangeFeedProcessorClassName}() {{ }}

        public {dbModel.ChangeFeedProcessorClassName}(
            Container databaseContainer,
            Container leaseContainer,
            string processorName,
            {dbModel.ChangeFeedHandlersClassName} {dbModel.ChangeFeedHandlersClassName.Parameterify()},
            int maxItemsPerBatch = DefaultMaxItemsPerBatch,
            TimeSpan? pollInterval = null,
            DateTime? startTime = null) 
            : base (
                processorName: processorName,
                maxItemsPerBatch: maxItemsPerBatch,
                pollInterval: pollInterval,
                startTime: startTime,
                databaseContainer: databaseContainer,
                leaseContainer: leaseContainer,
                changeFeedHandlers: {dbModel.ChangeFeedHandlersClassName.Parameterify()})
        {{
            this.{dbModel.ChangeFeedHandlersClassName} = {dbModel.ChangeFeedHandlersClassName.Parameterify()} ?? throw new ArgumentNullException(nameof({dbModel.ChangeFeedHandlersClassName.Parameterify()}));

            {dbModel.ChangeFeedHandlersClassName.Parameterify()}.ThrowIfAnyDocumentHandlerNotSet();
        }}

        protected override Task? GetHandlerTask(DbDoc doc, CancellationToken cancellationToken) => doc switch
        {{
{string.Concat(dbModel.Partitions.Values.SelectMany(x=>x.Documents.Values).Select(CallHandler))}
            _ => throw new NotSupportedException($""Document of type {{doc?.GetType().Name}} was unexpected"")
        }};
    }}
}}
";

            context.AddSource($"feed_{dbModel.ChangeFeedProcessorClassName}.cs", s);
        }

        static string CallHandler(DbDocumentModel documentModel) => $@"
            {documentModel.ClassFullName} x => {documentModel.DbPartitionModel.DbModel.ChangeFeedHandlersClassName}.{documentModel.DbPartitionModel.Name}.{documentModel.ClassName}?.Invoke(x, cancellationToken),";
    }
}
