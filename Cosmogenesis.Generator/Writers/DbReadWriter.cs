using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class DbReadWriter
    {
        public static void Write(GeneratorExecutionContext context, DbModel dbModel)
        {
            var s = $@"
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace {dbModel.Namespace}
{{
    public class {dbModel.ReadClassName}
    {{
        protected virtual {dbModel.DbClassName} {dbModel.DbClassName} {{ get; }} = default!;

        /// <summary>Mocking constructor</summary>
        protected {dbModel.ReadClassName}() {{ }}

        protected internal {dbModel.ReadClassName}({dbModel.DbClassName} {dbModel.DbClassName.Parameterify()})
        {{
            this.{dbModel.DbClassName} = {dbModel.DbClassName.Parameterify()} ?? throw new ArgumentNullException(nameof({dbModel.DbClassName.Parameterify()}));
        }}

{string.Concat(dbModel.Partitions.Values.SelectMany(x=>x.Documents.Values).Select(Read))}
    }}
}}
";

            context.AddSource($"db_{dbModel.ReadClassName}.cs", s);
        }

        static string Read(DbDocumentModel documentModel) => $@"
        /// <summary>
        /// Try to load a {documentModel.ClassName} by pk & id.
        /// id should be transformed using DbDocHelper.GetValidId.
        /// Returns the {documentModel.ClassName} or null if not found.
        /// </summary>
        /// <exception cref=""DbOverloadedException"" />
        /// <exception cref=""DbUnknownStatusCodeException"" />
        public virtual Task<{documentModel.ClassFullName}?> {documentModel.ClassName}ByIdAsync(string partitionKey, string id) => 
            {documentModel.DbPartitionModel.DbModel.DbClassName}.ReadByIdAsync<{documentModel.ClassFullName}>(partitionKey: new PartitionKey(partitionKey), id: id, type: {documentModel.ConstDocType});
";
    }
}
