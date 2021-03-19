using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class PartitionQueryBuilderWriter
    {
        public static void Write(GeneratorExecutionContext context, DbPartitionModel partitionModel)
        {
            var s = $@"
using System.Linq;
using Cosmogenesis.Core;
using Microsoft.Azure.Cosmos;

namespace {partitionModel.DbModel.Namespace}
{{
    public class {partitionModel.QueryBuilderClassName} : DbQueryBuilderBase
    {{
        /// <summary>Mocking constructor</summary>
        protected {partitionModel.QueryBuilderClassName}() {{ }}

        protected internal {partitionModel.QueryBuilderClassName}(
            {partitionModel.DbModel.DbClassName} {partitionModel.DbModel.DbClassName.Parameterify()},
            PartitionKey? partitionKey)
            : base(
                dbBase: {partitionModel.DbModel.DbClassName.Parameterify()},
                partitionKey: partitionKey)
        {{
        }}

{string.Concat(partitionModel.Documents.Values.Select(BuildQuery))}
    }}
}}
";

            context.AddSource($"partition_{partitionModel.QueryBuilderClassName}.cs", s);
        }

        static string BuildQuery(DbDocumentModel documentModel) => $@"
        /// <summary>
        /// Build a query filtered to {documentModel.ClassName} documents.
        /// Additional Linq transformations can be appended.
        /// Use ExecuteQueryAsync to execute.
        /// <see cref=""https://github.com/Azure/azure-cosmos-dotnet-v3/blob/bb72ba5786d99d928b4774e16810f2655029e8a2/Microsoft.Azure.Cosmos/src/Linq/CosmosLinqExtensions.cs"" />
        /// </summary>
        public virtual IQueryable<{documentModel.ClassFullName}> {documentModel.Name.Pluralize()}() => 
            BuildQueryByType<{documentModel.ClassFullName}>(type: {documentModel.ConstDocType});
";
            
    }
}
