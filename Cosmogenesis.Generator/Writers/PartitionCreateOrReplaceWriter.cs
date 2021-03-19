using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class PartitionCreateOrReplaceWriter
    {
        public static void Write(GeneratorExecutionContext context, DbPartitionModel partitionModel)
        {
            if (!partitionModel.Documents.Values.Any(x => x.IsMutable || x.IsTransient))
            {
                return;
            }

            var s = $@"
using System;
using System.Threading.Tasks;
using Cosmogenesis.Core;

namespace {partitionModel.DbModel.Namespace}
{{
    public class {partitionModel.CreateOrReplaceClassName}
    {{
        protected virtual {partitionModel.ClassName} {partitionModel.ClassName} {{ get; }} = default!;

        /// <summary>Mocking constructor</summary>
        protected {partitionModel.CreateOrReplaceClassName}() {{ }}

        protected internal {partitionModel.CreateOrReplaceClassName}({partitionModel.ClassName} {partitionModel.ClassName.Parameterify()})
        {{
            this.{partitionModel.ClassName} = {partitionModel.ClassName.Parameterify()} ?? throw new ArgumentNullException(nameof({partitionModel.ClassName.Parameterify()}));
        }}

{string.Concat(partitionModel.Documents.Values.Select(CreateOrReplace))}
    }}
}}
";

            context.AddSource($"partition_{partitionModel.CreateOrReplaceClassName}.cs", s);
        }

        static string CreateOrReplace(DbDocumentModel documentModel) => 
            !documentModel.IsTransient && !documentModel.IsMutable
            ? ""
            : $@"
        /// <summary>
        /// Create or replace (unconditionally overwrite) a {documentModel.ClassName}.
        /// </summary>
        /// <exception cref=""DbOverloadedException"" />
        /// <exception cref=""DbUnknownStatusCodeException"" />
        public virtual Task<CreateOrReplaceResult<{documentModel.ClassFullName}>> {documentModel.ClassName}Async({documentModel.PropertiesAsInputParameters}) => 
            {documentModel.DbPartitionModel.ClassName}.CreateOrReplaceAsync(new {documentModel.ClassFullName} {{ {documentModel.PropertiesAsSetters} }});
";
    }
}
