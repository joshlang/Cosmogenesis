using Cosmogenesis.Generator.Models;
using Cosmogenesis.Generator.Models.Attributes;
using Cosmogenesis.Generator.Plans;

namespace Cosmogenesis.Generator.PlanBuilders;
static class DatabasePlanBuilder
{
    public static void Build(OutputModel outputModel, OutputPlan outputPlan)
    {
        if (!outputModel.CanGenerate) { return; }

        foreach (var classModel in outputModel.Classes)
        {
            outputModel.CancellationToken.ThrowIfCancellationRequested();
            outputPlan.DatabasePlansByClass[classModel] = new();
            foreach (var dbAttribute in classModel.DbAttributes)
            {
                Initialize(outputModel, classModel, outputPlan, dbAttribute);
            }
        }
        var defaultDatabase = outputPlan.DatabasePlansByName.Count == 1 ? outputPlan.DatabasePlansByName.Values.Single() : null;
        if (defaultDatabase is not null)
        {
            foreach (var byClass in outputPlan.DatabasePlansByClass.Values.Where(x => x.Count == 0))
            {
                byClass.Add(defaultDatabase);
            }
        }
        foreach (var classModel in outputPlan.DatabasePlansByName.Values.GroupBy(x => x.Namespace).Where(x => x.Count() > 1).Select(x => x.First().ClassModel))
        {
            outputModel.Report(Diagnostics.Errors.DatabaseNamespaces, classModel.ClassSymbol);
        }

        PartitionPlanBuilder.AddPartitions(outputModel, outputPlan);
        DocumentPlanBuilder.AddDocuments(outputModel, outputPlan);
        PartitionPlanBuilder.RemoveEmptyPartitions(outputModel, outputPlan);
    }
    static void Initialize(OutputModel outputModel, ClassModel classModel, OutputPlan outputPlan, DbAttributeModel dbAttribute)
    {
        var name = dbAttribute.Name ?? "";
        if (outputPlan.DatabasePlansByName.TryGetValue(name, out var plan))
        {
            if (dbAttribute.Namespace.NullIfEmpty() is not null &&
                dbAttribute.Namespace != plan.Namespace)
            {
                if (plan.IsDefaultNamespace)
                {
                    plan.IsDefaultNamespace = false;
                    plan.Namespace = dbAttribute.Namespace!;
                    plan.ClassModel = classModel;
                }
                else
                {
                    outputModel.Report(Diagnostics.Errors.DbMultipleNamespace, classModel.ClassSymbol);
                }
            }
        }
        else
        {
            plan = new DatabasePlan
            {
                Namespace = dbAttribute.Namespace.NullIfEmpty()!,
                Name = name,
                ClassModel = classModel,
                DbClassName = name.WithSuffix(Suffixes.Database),
                PartitionsClassName = name.WithSuffix(Suffixes.Partitions),
                QueryBuilderClassName = name.WithSuffix(Suffixes.QueryBuilder),
                QueryClassName = name.WithSuffix(Suffixes.Query),
                ReadClassName = name.WithSuffix(Suffixes.Read),
                SerializerClassName = name.WithSuffix(Suffixes.Serializer),
                ConverterClassName = name.WithSuffix(Suffixes.Converter),
                TypesClassName = name.WithSuffix(Suffixes.Types),
                ChangeFeedHandlersClassName = name.WithSuffix(Suffixes.ChangeFeedHandlers),
                ChangeFeedProcessorClassName = name.WithSuffix(Suffixes.ChangeFeedProcessor)                
            };
            plan.ChangeFeedHandlersArgumentName = plan.ChangeFeedHandlersClassName.ToArgumentName();
            plan.IsDefaultNamespace = plan.Namespace is null;
            plan.Namespace ??= classModel.ClassSymbol.ContainingNamespace.ToDisplayString();
            plan.DbClassNameArgument = plan.DbClassName.ToArgumentName();
            plan.QueryBuilderClassNameArgument = plan.QueryBuilderClassName.ToArgumentName();
            outputModel.ValidateNames(
                classModel.ClassSymbol,
                plan.Name,
                plan.DbClassName,
                plan.PartitionsClassName,
                plan.QueryBuilderClassName,
                plan.QueryClassName,
                plan.ReadClassName,
                plan.SerializerClassName,
                plan.ConverterClassName,
                plan.TypesClassName,
                plan.ChangeFeedHandlersClassName,
                plan.ChangeFeedProcessorClassName);
            outputModel.ValidateIdentifiers(
                classModel.ClassSymbol,
                plan.ChangeFeedHandlersArgumentName,
                plan.DbClassNameArgument,
                plan.QueryBuilderClassNameArgument);
            plan.Namespace.ValidateNamespace(outputModel, classModel.ClassSymbol);
            outputPlan.DatabasePlansByName[name] = plan;
        }
        outputPlan.DatabasePlansByClass[classModel].Add(plan);
    }
}
