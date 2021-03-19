using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class PartitionWriter
    {
        public static void Write(GeneratorExecutionContext context, DbPartitionModel partitionModel)
        {
            var s = $@"
using System;
using System.Threading.Tasks;
using Cosmogenesis.Core;

namespace {partitionModel.DbModel.Namespace}
{{
    public class {partitionModel.ClassName} : DbPartitionBase
    {{
        protected virtual {partitionModel.DbModel.DbClassName} {partitionModel.DbModel.DbClassName} {{ get; }} = default!;

        /// <summary>Mocking constructor</summary>
        protected {partitionModel.ClassName}() {{ }}

        protected internal {partitionModel.ClassName}(
            {partitionModel.DbModel.DbClassName} {partitionModel.DbModel.DbClassName.Parameterify()},
            string partitionKey)
            : base(
                db: {partitionModel.DbModel.DbClassName.Parameterify()},
                partitionKey: partitionKey,
                serializer: {partitionModel.DbModel.SerializerClassName}.Instance)
        {{
            this.{partitionModel.DbModel.DbClassName} = {partitionModel.DbModel.DbClassName.Parameterify()} ?? throw new ArgumentNullException(nameof({partitionModel.DbModel.DbClassName.Parameterify()}));
        }}

        {partitionModel.QueryBuilderClassName}? queryBuilder;
        /// <summary>
        /// Methods to build queries for later execution.
        /// </summary>
        public virtual {partitionModel.QueryBuilderClassName} QueryBuilder => queryBuilder ??= new {partitionModel.QueryBuilderClassName}(
            {partitionModel.DbModel.DbClassName.Parameterify()}: {partitionModel.DbModel.DbClassName},
            partitionKey: PartitionKey);

        {partitionModel.QueryClassName}? query;
        /// <summary>
        /// Methods to execute queries.
        /// </summary>
        public virtual {partitionModel.QueryClassName} Query => query ??= new {partitionModel.QueryClassName}(
            {partitionModel.DbModel.DbClassName.Parameterify()}: {partitionModel.DbModel.DbClassName},
            {partitionModel.QueryBuilderClassName.Parameterify()}: QueryBuilder);

        /// <summary>
        /// A batch of operations to be executed atomically (or not at all) within a {partitionModel.Name} in the {partitionModel.DbModel.Name} database.
        /// </summary>
        public virtual {partitionModel.BatchClassName} CreateBatch() => new {partitionModel.BatchClassName}(
            transactionalBatch: CreateBatchForPartition(),
            partitionKey: PartitionKeyString,
            validateStateBeforeSave: {partitionModel.DbModel.DbClassName}.ValidateStateBeforeSave);

        {partitionModel.ReadClassName}? read;
        /// <summary>
        /// Methods to read documents.
        /// </summary>
        public virtual {partitionModel.ReadClassName} Read => read ??= new {partitionModel.ReadClassName}(
            {partitionModel.DbModel.DbClassName.Parameterify()}: {partitionModel.DbModel.DbClassName}, 
            partitionKey: PartitionKey);

        {partitionModel.ReadOrThrowClassName}? readOrThrow;
        /// <summary>
        /// Methods to read documents, or throw DbConflictException is they are not found.
        /// </summary>
        public virtual {partitionModel.ReadOrThrowClassName} ReadOrThrow => readOrThrow ??= new {partitionModel.ReadOrThrowClassName}(
            {partitionModel.DbModel.DbClassName.Parameterify()}: {partitionModel.DbModel.DbClassName}, 
            partitionKey: PartitionKey);

        {partitionModel.CreateClassName}? create;
        /// <summary>
        /// Methods to create documents.
        /// </summary>
        public virtual {partitionModel.CreateClassName} Create => create ??= new {partitionModel.CreateClassName}({partitionModel.ClassName.Parameterify()}: this);

        {partitionModel.ReadOrCreateClassName}? readOrCreate;
        /// <summary>
        /// Methods to read documents, or create them if they did not yet exist.
        /// </summary>
        public virtual {partitionModel.ReadOrCreateClassName} ReadOrCreate => readOrCreate ??= new {partitionModel.ReadOrCreateClassName}({partitionModel.ClassName.Parameterify()}: this);

{ReadMany(partitionModel)}
{CreateOrReplace(partitionModel)}
{string.Concat(partitionModel.Documents.Values.Select(Create))}
{string.Concat(partitionModel.Documents.Values.Select(CreateOrReplace))}
{string.Concat(partitionModel.Documents.Values.Select(ReadOrCreate))}
{string.Concat(partitionModel.Documents.Values.Select(ReplaceIfMutable))}
{string.Concat(partitionModel.Documents.Values.Select(DeleteIfTransient))}
    }}
}}
";

            context.AddSource($"partition_{partitionModel.ClassName}.cs", s);

            BatchWriter.Write(context, partitionModel);
            PartitionCreateWriter.Write(context, partitionModel);
            PartitionReadOrCreateWriter.Write(context, partitionModel);
            PartitionCreateOrReplaceWriter.Write(context, partitionModel);
            PartitionReadWriter.Write(context, partitionModel);
            PartitionReadOrThrowWriter.Write(context, partitionModel);
            PartitionReadManyWriter.Write(context, partitionModel);
            PartitionQueryBuilderWriter.Write(context, partitionModel);
            PartitionQueryWriter.Write(context, partitionModel);
        }

        static string ReadMany(DbPartitionModel partitionModel) =>
            !partitionModel.Documents.Values.Any(x => x.GetIdModel.HasParameters)
            ? ""
            : $@"
        {partitionModel.ReadManyClassName}? readMany;
        /// <summary>
        /// Methods to read multiple documents at once, though not necessarily in a single operation.
        /// </summary>
        public virtual {partitionModel.ReadManyClassName} ReadMany => readMany ??= new {partitionModel.ReadManyClassName}(
            {partitionModel.DbModel.DbClassName.Parameterify()}: {partitionModel.DbModel.DbClassName}, 
            partitionKey: PartitionKey);
";

        static string CreateOrReplace(DbPartitionModel partitionModel) =>
            !partitionModel.Documents.Values.Any(x => x.IsTransient || x.IsMutable)
            ? ""
            : $@"
        {partitionModel.CreateOrReplaceClassName}? createOrReplace;
        /// <summary>
        /// Methods to create or replace (unconditionally overwrite) documents.
        /// </summary>
        public virtual {partitionModel.CreateOrReplaceClassName} CreateOrReplace => createOrReplace ??= new {partitionModel.CreateOrReplaceClassName}({partitionModel.ClassName.Parameterify()}: this);
";

        static string ReplaceIfMutable(DbDocumentModel documentModel) =>
            !documentModel.IsMutable
            ? ""
            : $@"
        /// <summary>
        /// Try to replace an existing {documentModel.ClassName}.
        /// </summary>
        /// <exception cref=""DbOverloadedException"" />
        /// <exception cref=""DbUnknownStatusCodeException"" />
        public virtual Task<ReplaceResult<{documentModel.ClassFullName}>> ReplaceAsync({documentModel.ClassFullName} {documentModel.ClassName.Parameterify()}) =>
            ReplaceItemAsync(
                item: {documentModel.ClassName.Parameterify()} ?? throw new ArgumentNullException(nameof({documentModel.ClassName.Parameterify()})), 
                type: {documentModel.ConstDocType});
";

        static string DeleteIfTransient(DbDocumentModel documentModel) =>
            !documentModel.IsTransient
            ? ""
            : $@"
        /// <summary>
        /// Try to delete an existing {documentModel.ClassName}.
        /// </summary>
        /// <exception cref=""DbOverloadedException"" />
        /// <exception cref=""DbUnknownStatusCodeException"" />
        public virtual Task<DbConflictType?> DeleteAsync({documentModel.ClassFullName} {documentModel.ClassName.Parameterify()}) =>
            DeleteItemAsync(
                item: {documentModel.ClassName.Parameterify()} ?? throw new ArgumentNullException(nameof({documentModel.ClassName.Parameterify()})));
";

        static string Create(DbDocumentModel documentModel) => $@"
        /// <summary>
        /// Try to create a {documentModel.ClassName}.
        /// .id must be set if there is no stable id generator defined
        /// .pk, .CreationDate and .Type are set automatically
        /// </summary>
        /// <exception cref=""DbOverloadedException"" />
        /// <exception cref=""DbUnknownStatusCodeException"" />
        protected internal virtual Task<CreateResult<{documentModel.ClassFullName}>> CreateAsync({documentModel.ClassFullName} {documentModel.ClassName.Parameterify()})
        {{
            {DocumentModelWriter.CreateAndCheckPkAndId(documentModel, documentModel.ClassName.Parameterify())}
            return CreateItemAsync(item: {documentModel.ClassName.Parameterify()}, type: {documentModel.ConstDocType});
        }}
";

        static string CreateOrReplace(DbDocumentModel documentModel) =>
            !documentModel.IsMutable && !documentModel.IsTransient
            ? ""
            : $@"
        /// <summary>
        /// Create or replace (unconditionally overwrite) a {documentModel.ClassName}.
        /// .id must be set if there is no stable id generator defined
        /// .pk, .CreationDate and .Type are set automatically
        /// </summary>
        /// <exception cref=""DbOverloadedException"" />
        /// <exception cref=""DbUnknownStatusCodeException"" />
        protected internal virtual Task<CreateOrReplaceResult<{documentModel.ClassFullName}>> CreateOrReplaceAsync({documentModel.ClassFullName} {documentModel.ClassName.Parameterify()})
        {{
            {DocumentModelWriter.CreateAndCheckPkAndId(documentModel, documentModel.ClassName.Parameterify())}
            return CreateOrReplaceItemAsync(item: {documentModel.ClassName.Parameterify()}, type: {documentModel.ConstDocType});
        }}
";

        static string ReadOrCreate(DbDocumentModel documentModel) => $@"
        /// <summary>
        /// Read a {documentModel.ClassName} document, or create it if it does not yet exist.
        /// .id must be set if there is no stable id generator defined
        /// .pk, .CreationDate and .Type are set automatically
        /// </summary>
        /// <exception cref=""DbOverloadedException"" />
        /// <exception cref=""DbUnknownStatusCodeException"" />
        protected internal virtual Task<ReadOrCreateResult<{documentModel.ClassFullName}>> ReadOrCreateAsync(bool tryCreateFirst, {documentModel.ClassFullName} {documentModel.ClassName.Parameterify()})
        {{
            {DocumentModelWriter.CreateAndCheckPkAndId(documentModel, documentModel.ClassName.Parameterify())}
            return ReadOrCreateItemAsync(item: {documentModel.ClassName.Parameterify()}, type: {documentModel.ConstDocType}, tryCreateFirst: tryCreateFirst);
        }}
";
    }
}
