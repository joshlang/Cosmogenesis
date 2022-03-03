﻿using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.Writers;
static class DbReadWriter
{
    public static void Write(OutputModel outputModel, DatabasePlan databasePlan)
    {
        var s = $@"
namespace {databasePlan.Namespace};

public class {databasePlan.ReadClassName}
{{
    protected virtual {databasePlan.Namespace}.{databasePlan.DbClassName} {databasePlan.DbClassName} {{ get; }} = default!;

    /// <summary>Mocking constructor</summary>
    protected {databasePlan.ReadClassName}() {{ }}

    internal protected {databasePlan.ReadClassName}({databasePlan.Namespace}.{databasePlan.DbClassName} {databasePlan.DbClassNameArgument})
    {{
        this.{databasePlan.DbClassName} = {databasePlan.DbClassNameArgument} ?? throw new System.ArgumentNullException(nameof({databasePlan.DbClassNameArgument}));
    }}

{string.Concat(databasePlan.PartitionPlansByName.Values.SelectMany(x => x.DocumentsByDocType.Values).Select(x => Read(databasePlan, x)))}
}}
";

        outputModel.Context.AddSource($"db_{databasePlan.ReadClassName}.cs", s);
    }

    static string Read(DatabasePlan databasePlan, DocumentPlan documentPlan) => $@"
    /// <summary>
    /// Try to load a {documentPlan.ClassName} by pk & id.
    /// id should be transformed using DbDocHelper.GetValidId.
    /// Returns the {documentPlan.ClassName} or null if not found.
    /// </summary>
    /// <exception cref=""DbOverloadedException"" />
    /// <exception cref=""DbUnknownStatusCodeException"" />
    public virtual System.Threading.Tasks.Task<{documentPlan.FullTypeName}?> {documentPlan.ClassName}ByIdAsync(string partitionKey, string id) => 
        this.{databasePlan.DbClassName}.ReadByIdAsync<{documentPlan.FullTypeName}>(
            partitionKey: new Microsoft.Azure.Cosmos.PartitionKey(partitionKey), 
            id: id, 
            type: {documentPlan.ConstDocType});
";

}
