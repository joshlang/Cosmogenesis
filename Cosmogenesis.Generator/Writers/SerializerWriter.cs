using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class SerializerWriter
    {
        public static void Write(GeneratorExecutionContext context, DbModel dbModel)
        {
            var s = $@"
using System;
using System.Text.Json;
using Cosmogenesis.Core;

namespace {dbModel.Namespace}
{{
    /// <summary>
    /// This serializer knows how to handle all the documents in the {dbModel.Name} database.
    /// </summary>
    public class {dbModel.SerializerClassName} : DbSerializerBase
    {{
        /// <summary>
        /// This serializer knows how to handle all the documents in the {dbModel.Name} database.
        /// </summary>
        public static readonly {dbModel.SerializerClassName} Instance = new();

        protected {dbModel.SerializerClassName}()
        {{
            DeserializeOptions.Converters.Add({dbModel.ConverterClassName}.Instance);
        }}

        protected override DbDoc? DeserializeByType(ReadOnlySpan<byte> data, string? type) => type switch
        {{
{string.Concat(dbModel.Partitions.Values.SelectMany(x => x.Documents.Values).Select(DeserializeType).Select(x => $"{x},"))}
            _ => throw new NotSupportedException($""We don't know how to deserialize a message of type {{type}}"")
        }};
    }}
}}
";

            context.AddSource($"db_{dbModel.SerializerClassName}.cs", s);
        }

        static string DeserializeType(DbDocumentModel documentModel) => $@"
            {documentModel.ConstDocType} => JsonSerializer.Deserialize<{documentModel.ClassFullName}>(data, DeserializeOptions)";
    }
}
