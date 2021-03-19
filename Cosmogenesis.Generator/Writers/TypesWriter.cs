using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class TypesWriter
    {
        public static void Write(GeneratorExecutionContext context, DbModel dbModel)
        {
            var s = $@"
namespace {dbModel.Namespace}
{{
    public static class {dbModel.TypesClassName}
    {{
{string.Concat(dbModel.Partitions.Values.Select(PartitionTypes))}
    }}
}}
";

            context.AddSource($"db_{dbModel.TypesClassName}.cs", s);
        }

        static string PartitionTypes(DbPartitionModel partitionModel) => $@"
        public static class {partitionModel.ClassName}
        {{
{string.Concat(partitionModel.Documents.Values.Select(Type))}
        }}
";

        static string Type(DbDocumentModel documentModel) => $@"
            public const string {documentModel.ClassName} = ""{documentModel.TypeId}"";";
    }
}
