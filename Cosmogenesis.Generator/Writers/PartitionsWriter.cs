using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Writers
{
    class PartitionsWriter
    {
        public static void Write(GeneratorExecutionContext context, DbModel dbModel)
        {
            var s = $@"
using System;

namespace {dbModel.Namespace}
{{
    public class {dbModel.DbPartitionsClassName}
    {{
        protected virtual {dbModel.DbClassName} {dbModel.DbClassName} {{ get; }} = default!;

        /// <summary>Mocking constructor</summary>
        protected {dbModel.DbPartitionsClassName}() {{ }}

        protected internal {dbModel.DbPartitionsClassName}({dbModel.DbClassName} {dbModel.DbClassName.Parameterify()})
        {{
            this.{dbModel.DbClassName} = {dbModel.DbClassName.Parameterify()} ?? throw new ArgumentNullException(nameof({dbModel.DbClassName.Parameterify()}));
        }}

{string.Concat(dbModel.Partitions.Values.Select(Partition))}
    }}
}}
";
            context.AddSource($"db_{dbModel.DbPartitionsClassName}.cs", s);

            foreach (var partition in dbModel.Partitions.Values)
            {
                PartitionWriter.Write(context, partition);
            }
        }

        static string Partition(DbPartitionModel partitionModel) => $@"
        public virtual {partitionModel.ClassName} {partitionModel.Name}({partitionModel.GetKeyModel.InputParameters}) =>
            new {partitionModel.ClassName}(
                {partitionModel.DbModel.DbClassName.Parameterify()}: {partitionModel.DbModel.DbClassName},
                partitionKey: {partitionModel.GetKeyModel.FullMethodName}({partitionModel.GetKeyModel.InputParameterMapping}));
";
    }
}
