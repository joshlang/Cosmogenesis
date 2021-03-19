using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class DbQueryWriter
    {
        public static void Write(GeneratorExecutionContext context, DbModel dbModel)
        {
            var s = $@"
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Cosmogenesis.Core;

namespace {dbModel.Namespace}
{{
    public class {dbModel.QueryClassName} : DbQueryBase
    {{
        protected virtual {dbModel.DbClassName} {dbModel.DbClassName} {{ get; }} = default!;
        protected virtual {dbModel.QueryBuilderClassName} {dbModel.QueryBuilderClassName} {{ get; }} = default!;

        /// <summary>Mocking constructor</summary>
        protected {dbModel.QueryClassName}() {{ }}

        protected internal {dbModel.QueryClassName}(
            {dbModel.DbClassName} {dbModel.DbClassName.Parameterify()},
            {dbModel.QueryBuilderClassName} {dbModel.QueryBuilderClassName.Parameterify()})
            : base(
                dbBase: {dbModel.DbClassName.Parameterify()},
                queryBuilder: {dbModel.QueryBuilderClassName.Parameterify()})
        {{
            this.{dbModel.DbClassName} = {dbModel.DbClassName.Parameterify()} ?? throw new ArgumentNullException(nameof({dbModel.DbClassName.Parameterify()}));
            this.{dbModel.QueryBuilderClassName} = {dbModel.QueryBuilderClassName.Parameterify()} ?? throw new ArgumentNullException(nameof({dbModel.QueryBuilderClassName.Parameterify()}));
        }}

{string.Concat(dbModel.Partitions.Values.SelectMany(x=>x.Documents.Values).Select(Query))}
    }}
}}
";

            context.AddSource($"db_{dbModel.QueryClassName}.cs", s);
        }

        static string Query(DbDocumentModel documentModel) => $@"
        /// <summary>
        /// Build and execute a query filtered to {documentModel.ClassName} documents.
        /// <see cref=""https://github.com/Azure/azure-cosmos-dotnet-v3/blob/bb72ba5786d99d928b4774e16810f2655029e8a2/Microsoft.Azure.Cosmos/src/Linq/CosmosLinqExtensions.cs"" />
        /// </summary>
        public virtual IAsyncEnumerable<T> {documentModel.ClassName.Pluralize()}<T>(
            Func<IQueryable<{documentModel.ClassFullName}>, IQueryable<T>> createQuery,
            CancellationToken cancellationToken = default) 
            => {documentModel.DbPartitionModel.DbModel.DbClassName}
                .ExecuteQueryAsync(
                    query: createQuery({documentModel.DbPartitionModel.DbModel.QueryBuilderClassName}.{documentModel.ClassName.Pluralize()}()),
                    cancellationToken: cancellationToken);

        /// <summary>
        /// Execute a query filtered to {documentModel.ClassName} documents.
        /// </summary>
        public virtual IAsyncEnumerable<{documentModel.ClassFullName}> {documentModel.ClassName.Pluralize()}(
            CancellationToken cancellationToken = default)
            => {documentModel.DbPartitionModel.DbModel.DbClassName}
                .ExecuteQueryAsync(
                    query: {documentModel.DbPartitionModel.DbModel.QueryBuilderClassName}.{documentModel.ClassName.Pluralize()}(),
                    cancellationToken: cancellationToken);
";
    }
}
