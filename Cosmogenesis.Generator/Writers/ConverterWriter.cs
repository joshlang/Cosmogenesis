using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class ConverterWriter
    {
        public static void Write(GeneratorExecutionContext context, DbModel dbModel)
        {
            var s = $@"
using System;
using Cosmogenesis.Core;
using System.Text.Json;

namespace {dbModel.Namespace}
{{
    /// <summary>
    /// This converter knows how to convert the DbDoc base type into documents from the {dbModel.Name} database.
    /// </summary>
    public class {dbModel.ConverterClassName} : DbDocConverterBase
    {{
        /// <summary>
        /// This converter knows how to convert the DbDoc base type into documents from the {dbModel.Name} database.
        /// </summary>
        public static readonly {dbModel.ConverterClassName} Instance = new();

        protected override DbDoc? DeserializeByType(ReadOnlySpan<byte> data, string? type, JsonSerializerOptions options) => type switch
        {{
{string.Concat(dbModel.Partitions.Values.SelectMany(x => x.Documents.Values).Select(DeserializeType).Select(x => $"{x},"))}            
            _ => throw new NotSupportedException($""We don't know how to deserialize a message of type {{type}}"")
        }};
    }}
}}
";

            context.AddSource($"db_{dbModel.ConverterClassName}.cs", s);
        }

        static string DeserializeType(DbDocumentModel documentModel) => $@"
            {documentModel.ConstDocType} => JsonSerializer.Deserialize<{documentModel.ClassFullName}>(utf8Json: data, options: options)";
    }
}
