using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class BatchWriter
    {
        public static void Write(GeneratorExecutionContext context, DbPartitionModel partitionModel)
        {
            var s = $@"
using System;
using Cosmogenesis.Core;
using Microsoft.Azure.Cosmos;

namespace {partitionModel.DbModel.Namespace}
{{
    public class {partitionModel.BatchClassName} : DbBatchBase
    {{
        /// <summary>Mocking constructor</summary>
        protected {partitionModel.BatchClassName}() {{ }}

        protected internal {partitionModel.BatchClassName}(
            TransactionalBatch transactionalBatch,
            string partitionKey,
            bool validateStateBeforeSave)
            : base(
                transactionalBatch: transactionalBatch,
                partitionKey: partitionKey,
                serializer: {partitionModel.DbModel.SerializerClassName}.Instance,
                validateStateBeforeSave: validateStateBeforeSave)
        {{
        }}

        /// <summary>
        /// Queue a document for creation in the batch.
        /// Throws InvalidOperationException if the DbDoc does not belong in the partition.
        /// </summary>
        public virtual {partitionModel.BatchClassName} CheckAndCreate(DbDoc dbDoc) => dbDoc switch
        {{
{string.Concat(partitionModel.Documents.Values.Select(CheckedCreate))}
            null => throw new ArgumentNullException(nameof(dbDoc)),
            _ => throw new InvalidOperationException($""{{dbDoc.GetType().Name}} is not a type stored in this partition"")
        }};

        /// <summary>
        /// Tries to queue a document for creation in the batch.
        /// Returns true if queued, or false if the document does not belong in the partition.
        /// </summary>
        public virtual bool TryCheckAndCreate(DbDoc dbDoc) => dbDoc switch
        {{
{string.Concat(partitionModel.Documents.Values.Select(CheckedCreate))}
            null => throw new ArgumentNullException(nameof(dbDoc)),
            _ => ({partitionModel.BatchClassName}?)null
        }} != null;

        /// <summary>
        /// Queue a document for creation or replacement in the batch.
        /// Throws InvalidOperationException if the DbDoc does not belong in the partition or is not mutable.
        /// </summary>
        public virtual {partitionModel.BatchClassName} CheckAndCreateOrReplace(DbDoc dbDoc) => dbDoc switch
        {{
{string.Concat(partitionModel.Documents.Values.Where(x => x.IsMutable || x.IsTransient).Select(CheckedCreateOrReplace))}
            null => throw new ArgumentNullException(nameof(dbDoc)),
            _ => throw new InvalidOperationException($""{{dbDoc.GetType().Name}} is not a mutable type in this partition"")
        }};

        /// <summary>
        /// Tries to queue a document for creation or replacement in the batch.
        /// Returns true if queued, or false if the document does not belong in the partition or is not mutable.
        /// </summary>
        public virtual bool TryCheckAndCreateOrReplace(DbDoc dbDoc) => dbDoc switch
        {{
{string.Concat(partitionModel.Documents.Values.Where(x => x.IsMutable || x.IsTransient).Select(CheckedCreateOrReplace))}
            null => throw new ArgumentNullException(nameof(dbDoc)),
            _ => ({partitionModel.BatchClassName}?)null
        }} != null;

        /// <summary>
        /// Queue a document for replacement in the batch.
        /// Throws InvalidOperationException if the DbDoc does not belong in the partition or is not mutable.
        /// </summary>
        public virtual {partitionModel.BatchClassName} CheckAndReplace(DbDoc dbDoc) => dbDoc switch
        {{
            {string.Concat(partitionModel.Documents.Values.Where(x => x.IsMutable).Select(CheckedReplace))}
            null => throw new ArgumentNullException(nameof(dbDoc)),
            _ => throw new InvalidOperationException($""{{dbDoc.GetType().Name}} is not a mutable type in this partition"")
        }};

        /// <summary>
        /// Tries to queue a document for replacement in the batch.
        /// Returns true if queued, or false if the document does not belong in the partition or is not mutable.
        /// </summary>
        public virtual bool TryCheckAndReplace(DbDoc dbDoc) => dbDoc switch
        {{
            {string.Concat(partitionModel.Documents.Values.Where(x => x.IsMutable).Select(CheckedReplace))}
            null => throw new ArgumentNullException(nameof(dbDoc)),
            _ => ({partitionModel.BatchClassName}?)null
        }} != null;

        /// <summary>
        /// Queue a document for deletion in the batch.
        /// Throws InvalidOperationException if the DbDoc does not belong in the partition or is not transient.
        /// </summary>
        public virtual {partitionModel.BatchClassName} CheckAndDelete(DbDoc dbDoc) => dbDoc switch
        {{
            {string.Concat(partitionModel.Documents.Values.Where(x => x.IsTransient).Select(CheckedDelete))}
            null => throw new ArgumentNullException(nameof(dbDoc)),
            _ => throw new InvalidOperationException($""{{dbDoc.GetType().Name}} is not a transient type in this partition"")
        }};

        /// <summary>
        /// Tries to queue a document for deletion in the batch.
        /// Returns true if queued, or false if the document does not belong in the partition or is not transient.
        /// </summary>
        public virtual bool TryCheckAndDelete(DbDoc dbDoc) => dbDoc switch
        {{
            {string.Concat(partitionModel.Documents.Values.Where(x => x.IsTransient).Select(CheckedDelete))}
            null => throw new ArgumentNullException(nameof(dbDoc)),
            _ => ({partitionModel.BatchClassName}?)null
        }} != null;

{string.Concat(partitionModel.Documents.Values.Select(Create))}
{string.Concat(partitionModel.Documents.Values.Select(CreateOrReplace))}
{string.Concat(partitionModel.Documents.Values.Select(Replace))}
{string.Concat(partitionModel.Documents.Values.Select(Delete))}
    }}
}}
";
            context.AddSource($"partition_{partitionModel.BatchClassName}.cs", s);
        }

        static string CheckedCreateOrReplace(DbDocumentModel documentModel) => $@"
            {documentModel.ClassFullName} x => CreateOrReplace({documentModel.ClassName.Parameterify()}: x),";

        static string CheckedCreate(DbDocumentModel documentModel) => $@"
            {documentModel.ClassFullName} x => Create({documentModel.ClassName.Parameterify()}: x),";
        
        static string CheckedReplace(DbDocumentModel documentModel) => $@"
            {documentModel.ClassFullName} x => Replace({documentModel.ClassName.Parameterify()}: x),";
        
        static string CheckedDelete(DbDocumentModel documentModel) => $@"
            {documentModel.ClassFullName} x => Delete({documentModel.ClassName.Parameterify()}: x),";

        static string Create(DbDocumentModel documentModel) => $@"
        /// <summary>
        /// Queue a {documentModel.ClassName} for creation in the batch
        /// </summary>
        protected virtual {documentModel.DbPartitionModel.BatchClassName} Create({documentModel.ClassFullName} {documentModel.ClassName.Parameterify()})
        {{
            {DocumentModelWriter.CreateAndCheckPkAndId(documentModel, documentModel.ClassName.Parameterify())}
            CreateCore(item: {documentModel.ClassName.Parameterify()}, type: {documentModel.ConstDocType});
            return this;
        }}

        /// <summary>
        /// Queue a {documentModel.ClassName} for creation in the batch
        /// </summary>
        public virtual {documentModel.DbPartitionModel.BatchClassName} Create{documentModel.ClassName}({documentModel.PropertiesAsInputParameters}) =>
            Create({documentModel.ClassName.Parameterify()}: new {documentModel.ClassFullName} {{ {documentModel.PropertiesAsSetters} }});
";

        static string CreateOrReplace(DbDocumentModel documentModel) =>
            !documentModel.IsTransient && !documentModel.IsMutable
            ? ""
            : $@"
        /// <summary>
        /// Queue a {documentModel.ClassName} for creation or replacement in the batch
        /// </summary>
        protected virtual {documentModel.DbPartitionModel.BatchClassName} CreateOrReplace({documentModel.ClassFullName} {documentModel.ClassName.Parameterify()})
        {{    
            {DocumentModelWriter.CreateAndCheckPkAndId(documentModel, documentModel.ClassName.Parameterify())}
            CreateOrReplaceCore(item: {documentModel.ClassName.Parameterify()}, type: {documentModel.ConstDocType});
            return this;
        }}

        /// <summary>
        /// Queue a {documentModel.ClassName} for creation or replacement in the batch
        /// </summary>
        public virtual {documentModel.DbPartitionModel.BatchClassName} CreateOrReplace{documentModel.ClassName}({documentModel.PropertiesAsInputParameters}) =>
            CreateOrReplace({documentModel.ClassName.Parameterify()}: new {documentModel.ClassFullName} {{ {documentModel.PropertiesAsSetters} }});
";

        static string Replace(DbDocumentModel documentModel) =>
            !documentModel.IsMutable
            ? ""
            : $@"
        /// <summary>
        /// Queue a {documentModel.ClassName} for replacement in the batch
        /// </summary>
        public virtual {documentModel.DbPartitionModel.BatchClassName} Replace({documentModel.ClassFullName} {documentModel.ClassName.Parameterify()})
        {{    
            ReplaceCore(item: {documentModel.ClassName.Parameterify()}, type: {documentModel.ConstDocType});
            return this;
        }}
";

        static string Delete(DbDocumentModel documentModel) =>
            !documentModel.IsTransient
            ? ""
            : $@"
        /// <summary>
        /// Queue a {documentModel.ClassName} for deletion in the batch
        /// </summary>
        public virtual {documentModel.DbPartitionModel.BatchClassName} Delete({documentModel.ClassFullName} {documentModel.ClassName.Parameterify()})
        {{
            DeleteCore(item: {documentModel.ClassName.Parameterify()});
            return this;
        }}
";
    }
}
