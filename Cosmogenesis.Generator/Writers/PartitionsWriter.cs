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

{PartitionGetters(dbModel)}
    }}
}}
";
            context.AddSource($"db_{dbModel.DbPartitionsClassName}.cs", s);

            foreach (var partition in dbModel.Partitions.Values)
            {
                PartitionWriter.Write(context, partition);
            }
        }

        static string PartitionGetters(DbModel dbModel)
        {
            var sb = new StringBuilder();
            foreach (var partition in dbModel.Partitions.Values)
            {
                sb.Append($@"
        public virtual {partition.ClassName} {partition.Name}({partition.GetKeyModel.InputParameters}) =>
            new {partition.ClassName}(
                {dbModel.DbClassName.Parameterify()}: {dbModel.DbClassName},
                partitionKey: {partition.GetKeyModel.FullMethodName}({partition.GetKeyModel.InputParameterMapping}));
");
            }
            return sb.ToString();
        }
    }
}
