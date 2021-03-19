using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class DbQueryBuilderWriter
    {
        public static void Write(GeneratorExecutionContext context, DbModel dbModel)
        {
            var s = $@"
using System.Linq;
using Cosmogenesis.Core;

namespace {dbModel.Namespace}
{{
    public class {dbModel.QueryBuilderClassName} : DbQueryBuilderBase
    {{
        /// <summary>Mocking constructor</summary>
        protected {dbModel.QueryBuilderClassName}() {{ }}

        protected internal {dbModel.QueryBuilderClassName}(
            {dbModel.DbClassName} {dbModel.DbClassName.Parameterify()})
            : base(
                dbBase: {dbModel.DbClassName.Parameterify()},
                partitionKey: null)
        {{
        }}

{string.Concat(dbModel.Partitions.Values.SelectMany(x=>x.Documents.Values).Select(BuildQuery))}
    }}
}}
";

            context.AddSource($"db_{dbModel.QueryBuilderClassName}.cs", s);
        }

        static string BuildQuery(DbDocumentModel documentModel) => $@"
        /// <summary>
        /// Build a query filtered to {documentModel.ClassName} documents.
        /// Additional Linq transformations can be appended.
        /// Use ExecuteQueryAsync to execute.
        /// <see cref=""https://github.com/Azure/azure-cosmos-dotnet-v3/blob/bb72ba5786d99d928b4774e16810f2655029e8a2/Microsoft.Azure.Cosmos/src/Linq/CosmosLinqExtensions.cs"" />
        /// </summary>
        public virtual IQueryable<{documentModel.ClassFullName}> {documentModel.ClassName.Pluralize()}() => 
            BuildQueryByType<{documentModel.ClassFullName}>(type: {documentModel.ConstDocType});
";
    }
}
