using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class PartitionReadOrCreateWriter
    {
        public static void Write(GeneratorExecutionContext context, DbPartitionModel partitionModel)
        {
            var s = $@"
using System;
using System.Threading.Tasks;
using Cosmogenesis.Core;

namespace {partitionModel.DbModel.Namespace}
{{
    public class {partitionModel.ReadOrCreateClassName}
    {{
        protected virtual {partitionModel.ClassName} {partitionModel.ClassName} {{ get; }} = default!;

        /// <summary>Mocking constructor</summary>
        protected {partitionModel.ReadOrCreateClassName}() {{ }}

        protected internal {partitionModel.ReadOrCreateClassName}({partitionModel.ClassName} {partitionModel.ClassName.Parameterify()})
        {{
            this.{partitionModel.ClassName} = {partitionModel.ClassName.Parameterify()} ?? throw new ArgumentNullException(nameof({partitionModel.ClassName.Parameterify()}));
        }}

{string.Concat(partitionModel.Documents.Values.Select(ReadOrCreate))}
    }}
}}
";

            context.AddSource($"partition_{partitionModel.ReadOrCreateClassName}.cs", s);
        }

        static string ReadOrCreate(DbDocumentModel documentModel) => $@"
        /// <summary>
        /// Read a {documentModel.ClassName} document, or create it if it does not yet exist.
        /// </summary>
        /// <exception cref=""DbOverloadedException"" />
        /// <exception cref=""DbUnknownStatusCodeException"" />
        public virtual Task<ReadOrCreateResult<{documentModel.ClassFullName}>> {documentModel.ClassName}Async({$"bool tryCreateFirst, {documentModel.PropertiesAsInputParameters}".TrimEnd(',', ' ')}) => 
            {documentModel.DbPartitionModel.ClassName}.ReadOrCreateAsync({documentModel.ClassName.Parameterify()}: new {documentModel.ClassFullName} {{ {documentModel.PropertiesAsSetters} }}, tryCreateFirst: tryCreateFirst);
";
    }
}
