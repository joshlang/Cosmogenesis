using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class DbWriter
    {
        public static void Write(GeneratorExecutionContext context, DbModel dbModel)
        {
            var s = $@"
using System;
using Microsoft.Azure.Cosmos;
using Cosmogenesis.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace {dbModel.Namespace}
{{
    public class {dbModel.DbClassName} : DbBase
    {{
        /// <summary>Mocking constructor</summary>
        protected {dbModel.DbClassName}() {{ }}
        
        public {dbModel.DbClassName}(Container container, DbSerializerBase? serializer = null, bool isReadOnly = false, bool validateStateBeforeSave = true)
            : base(
                container: container, 
                serializer: serializer ?? {dbModel.SerializerClassName}.Instance, 
                isReadOnly: isReadOnly, 
                validateStateBeforeSave: validateStateBeforeSave)
        {{
        }}

        {dbModel.DbPartitionsClassName}? partition;
        public virtual {dbModel.DbPartitionsClassName} Partition => partition ??= new {dbModel.DbPartitionsClassName}({dbModel.DbClassName.Parameterify()}: this);

        internal new Task<T?> ReadByIdAsync<T>(string id, PartitionKey partitionKey, string type) where T : DbDoc =>
            base.ReadByIdAsync<T>(id: id, partitionKey: partitionKey, type: type);

        internal new Task<T?[]> ReadByIdsAsync<T>(IEnumerable<string> ids, PartitionKey partitionKey, string type) where T : DbDoc =>
            base.ReadByIdsAsync<T>(ids: ids, partitionKey: partitionKey, type: type);

        {dbModel.QueryBuilderClassName}? crossPartitionQueryBuilder;
        /// <summary>
        /// Methods to build a cross-partition query for later execution.
        /// </summary>
        public virtual {dbModel.QueryBuilderClassName} CrossPartitionQueryBuilder => crossPartitionQueryBuilder ??= new {dbModel.QueryBuilderClassName}({dbModel.DbClassName.Parameterify()}: this);

        {dbModel.QueryClassName}? crossPartitionQuery;
        /// <summary>
        /// Methods to execute cross-partition queries.
        /// </summary>
        public virtual {dbModel.QueryClassName} CrossPartitionQuery => crossPartitionQuery ??= new {dbModel.QueryClassName}({dbModel.DbClassName.Parameterify()}: this, {dbModel.QueryBuilderClassName.Parameterify()}: CrossPartitionQueryBuilder);

        {dbModel.ReadClassName}? read;
        /// <summary>
        /// Methods to read documents by providing pk & id.
        /// </summary>
        public virtual {dbModel.ReadClassName} Read => read ??= new {dbModel.ReadClassName}({dbModel.DbClassName.Parameterify()}: this);
    }}
}}
";
            context.AddSource($"db_{dbModel.DbClassName}.cs", s);

            ConverterWriter.Write(context, dbModel);
            SerializerWriter.Write(context, dbModel);
            TypesWriter.Write(context, dbModel);

            DbReadWriter.Write(context, dbModel);
            DbQueryWriter.Write(context, dbModel);
            DbQueryBuilderWriter.Write(context, dbModel);

            PartitionsWriter.Write(context, dbModel);

            ChangeFeedHandlersWriter.Write(context, dbModel);
            ChangeFeedProcessorWriter.Write(context, dbModel);
        }
    }
}
