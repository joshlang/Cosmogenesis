using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers;
static class DatabasePlanWriter
{
    public static void Write(OutputModel outputModel, DatabasePlan databasePlan)
    {
        var s = $@"
namespace {databasePlan.Namespace};

public class {databasePlan.DbClassName} : Cosmogenesis.Core.DbBase
{{
    /// <summary>Mocking constructor</summary>
    protected {databasePlan.DbClassName}() {{ }}
        
    public {databasePlan.DbClassName}(
        Microsoft.Azure.Cosmos.Container container, 
        Cosmogenesis.Core.DbSerializerBase? serializer = null, 
        bool isReadOnly = false,
        bool validateStateBeforeSave = true)
        : base(
            container: container, 
            serializer: serializer ?? {databasePlan.Namespace}.{databasePlan.SerializerClassName}.Instance, 
            isReadOnly: isReadOnly, 
            validateStateBeforeSave: validateStateBeforeSave)
    {{
    }}

    {databasePlan.Namespace}.{databasePlan.PartitionsClassName}? partition;
    public virtual {databasePlan.Namespace}.{databasePlan.PartitionsClassName} Partition => this.partition ??= new(this);

    internal new System.Threading.Tasks.Task<T?> ReadByIdAsync<T>(
        string id, 
        Microsoft.Azure.Cosmos.PartitionKey partitionKey, 
        string type) where T : Cosmogenesis.Core.DbDoc =>
        base.ReadByIdAsync<T>(
            id: id, 
            partitionKey: partitionKey, 
            type: type);

    internal new System.Threading.Tasks.Task<T?[]> ReadByIdsAsync<T>(
        System.Collections.Generic.IEnumerable<string> ids, 
        Microsoft.Azure.Cosmos.PartitionKey partitionKey, 
        string type) where T : Cosmogenesis.Core.DbDoc =>
        base.ReadByIdsAsync<T>(
            ids: ids, 
            partitionKey: partitionKey, 
            type: type);

    {databasePlan.Namespace}.{databasePlan.QueryBuilderClassName}? crossPartitionQueryBuilder;
    /// <summary>
    /// Methods to build a cross-partition query for later execution.
    /// </summary>
    public virtual {databasePlan.Namespace}.{databasePlan.QueryBuilderClassName} CrossPartitionQueryBuilder => this.crossPartitionQueryBuilder ??= new(this);

    {databasePlan.Namespace}.{databasePlan.QueryClassName}? crossPartitionQuery;
    /// <summary>
    /// Methods to execute cross-partition queries.
    /// </summary>
    public virtual {databasePlan.Namespace}.{databasePlan.QueryClassName} CrossPartitionQuery => this.crossPartitionQuery ??= new(this, this.CrossPartitionQueryBuilder);

    {databasePlan.Namespace}.{databasePlan.ReadClassName}? read;
    /// <summary>
    /// Methods to read documents by providing pk & id.
    /// </summary>
    public virtual {databasePlan.Namespace}.{databasePlan.ReadClassName} Read => this.read ??= new(this);
}}
";

        outputModel.Context.AddSource($"db_{databasePlan.DbClassName}.cs", s);


        ConverterWriter.Write(outputModel, databasePlan);
        SerializerWriter.Write(outputModel, databasePlan);
        TypesWriter.Write(outputModel, databasePlan);

        DbReadWriter.Write(outputModel, databasePlan);
        DbQueryWriter.Write(outputModel, databasePlan);
        DbQueryBuilderWriter.Write(outputModel, databasePlan);

        PartitionsWriter.Write(outputModel, databasePlan);

        ChangeFeedHandlersWriter.Write(outputModel, databasePlan);
        ChangeFeedProcessorWriter.Write(outputModel, databasePlan);
    }
}
