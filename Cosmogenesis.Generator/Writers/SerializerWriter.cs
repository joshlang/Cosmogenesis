using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers;
static class SerializerWriter
{
    public static void Write(OutputModel outputModel, DatabasePlan databasePlan)
    {
        var s = $@"
namespace {databasePlan.Namespace};

/// <summary>
/// This serializer knows how to handle all the documents in the {databasePlan.Name} database.
/// </summary>
public class {databasePlan.SerializerClassName} : Cosmogenesis.Core.DbSerializerBase
{{
    /// <summary>
    /// This serializer knows how to handle all the documents in the {databasePlan.Name} database.
    /// </summary>
    public static readonly {databasePlan.Namespace}.{databasePlan.SerializerClassName} Instance = new();

    protected {databasePlan.SerializerClassName}()
    {{
        DeserializeOptions.Converters.Add({databasePlan.Namespace}.{databasePlan.ConverterClassName}.Instance);
    }}

    protected override Cosmogenesis.Core.DbDoc? DeserializeByType(
        System.ReadOnlySpan<byte> data, 
        string? type) => type switch
        {{
{string.Concat(databasePlan.PartitionPlansByName.Values.SelectMany(x => x.Documents).Select(DeserializeType))}
            _ => throw new System.NotSupportedException($""We don't know how to deserialize a message of type {{type}}"")
        }};
}}
";

        outputModel.Context.AddSource($"db_{databasePlan.SerializerClassName}.cs", s);
    }

    static string DeserializeType(DocumentPlan documentPlan) => $@"
            {documentPlan.ConstDocType} => System.Text.Json.JsonSerializer.Deserialize<{documentPlan.FullTypeName}>(data, this.DeserializeOptions),";
}
