using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class PartitionCreateWriter
    {
        public static void Write(GeneratorExecutionContext context, DbPartitionModel partitionModel)
        {
            var s = $@"
using System;
using System.Threading.Tasks;
using Cosmogenesis.Core;

namespace {partitionModel.DbModel.Namespace}
{{
    public class {partitionModel.CreateClassName}
    {{
        protected virtual {partitionModel.ClassName} {partitionModel.ClassName} {{ get; }} = default!;

        /// <summary>Mocking constructor</summary>
        protected {partitionModel.CreateClassName}() {{ }}

        protected internal {partitionModel.CreateClassName}({partitionModel.ClassName} {partitionModel.ClassName.Parameterify()})
        {{
            this.{partitionModel.ClassName} = {partitionModel.ClassName.Parameterify()} ?? throw new ArgumentNullException(nameof({partitionModel.ClassName.Parameterify()}));
        }}

{string.Concat(partitionModel.Documents.Values.Select(Create))}
    }}
}}
";

            context.AddSource($"partition_{partitionModel.CreateClassName}.cs", s);
        }

        static string Create(DbDocumentModel documentModel) => $@"
        /// <summary>
        /// Try to create a {documentModel.ClassName}.
        /// </summary>
        /// <exception cref=""DbOverloadedException"" />
        /// <exception cref=""DbUnknownStatusCodeException"" />
        public virtual Task<CreateResult<{documentModel.ClassFullName}>> {documentModel.ClassName}Async({documentModel.PropertiesAsInputParameters}) => 
            {documentModel.DbPartitionModel.ClassName}.CreateAsync({documentModel.ClassName.Parameterify()}: new {documentModel.ClassFullName} {{ {documentModel.PropertiesAsSetters} }});
";
    }
}
