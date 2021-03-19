using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class PartitionQueryWriter
    {
        public static void Write(GeneratorExecutionContext context, DbPartitionModel partitionModel)
        {
            var s = $@"
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System;
using Cosmogenesis.Core;

namespace {partitionModel.DbModel.Namespace}
{{
    public class {partitionModel.QueryClassName} : DbQueryBase
    {{
        protected virtual {partitionModel.DbModel.DbClassName} {partitionModel.DbModel.DbClassName} {{ get; }} = default!;
        protected virtual {partitionModel.QueryBuilderClassName} {partitionModel.QueryBuilderClassName} {{ get; }} = default!;

        /// <summary>Mocking constructor</summary>
        protected {partitionModel.QueryClassName}() {{ }}

        protected internal {partitionModel.QueryClassName}(
            {partitionModel.DbModel.DbClassName} {partitionModel.DbModel.DbClassName.Parameterify()},
            {partitionModel.QueryBuilderClassName} {partitionModel.QueryBuilderClassName.Parameterify()})
            : base(
                dbBase: {partitionModel.DbModel.DbClassName.Parameterify()},
                queryBuilder: {partitionModel.QueryBuilderClassName.Parameterify()})
        {{
            this.{partitionModel.DbModel.DbClassName} = {partitionModel.DbModel.DbClassName.Parameterify()} ?? throw new ArgumentNullException(nameof({partitionModel.DbModel.DbClassName.Parameterify()}));
            this.{partitionModel.QueryBuilderClassName} = {partitionModel.QueryBuilderClassName.Parameterify()} ?? throw new ArgumentNullException(nameof({partitionModel.QueryBuilderClassName.Parameterify()}));
        }}

{string.Concat(partitionModel.Documents.Values.Select(Query))}
    }}
}}
";

            context.AddSource($"partition_{partitionModel.QueryClassName}.cs", s);
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
                    query: createQuery({documentModel.DbPartitionModel.QueryBuilderClassName}.{documentModel.ClassName.Pluralize()}()),
                    cancellationToken: cancellationToken);

        /// <summary>
        /// Execute a query filtered to {documentModel.ClassName} documents.
        /// </summary>
        public virtual IAsyncEnumerable<{documentModel.ClassFullName}> {documentModel.ClassName.Pluralize()}(
            CancellationToken cancellationToken = default)
            => {documentModel.DbPartitionModel.DbModel.DbClassName}
                .ExecuteQueryAsync(
                    query: {documentModel.DbPartitionModel.QueryBuilderClassName}.{documentModel.ClassName.Pluralize()}(),
                    cancellationToken: cancellationToken);
";
    }
}
