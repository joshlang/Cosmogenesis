using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator.Models;
static class ModelValidation
{
    public static void Validate(OutputModel model)
    {
        model.CancellationToken.ThrowIfCancellationRequested();
        if (!model.CanGenerate) { return; }

        foreach (var classModel in model.Classes)
        {
            Validate(model, classModel);
        }
    }
    static void Validate(OutputModel outputModel, ClassModel model)
    {
        var symbol = model.ClassSymbol;
        foreach (var dbAttribute in model.DbAttributes)
        {
            dbAttribute.Name.ValidateName(outputModel, symbol);
        }
        if (model.PartitionAttribute is not null)
        {
            model.PartitionAttribute.Name.ValidateName(outputModel, symbol);
            if (!model.IsDbDoc)
            {
                outputModel.Report(Diagnostics.Errors.PartitionDbDoc, symbol);
            }
        }
        if (model.DocTypeAttribute is not null)
        {
            model.DocTypeAttribute.Name.ValidateName(outputModel, symbol);
            if (!model.IsDbDoc)
            {
                outputModel.Report(Diagnostics.Errors.DocTypeDbDoc, symbol);
            }
            if (symbol.IsAbstract)
            {
                outputModel.Report(Diagnostics.Errors.DocTypeAbstract, symbol);
            }
        }
        if (model.PartitionDefinitionAttribute is not null)
        {
            if (!symbol.IsStatic)
            {
                outputModel.Report(Diagnostics.Errors.PartitionDefinitionStatic, symbol);
            }
        }
        if (model.MutableAttribute is not null)
        {
            if (!model.IsDbDoc)
            {
                outputModel.Report(Diagnostics.Errors.MutableDbDoc, symbol);
            }
        }
        if (model.TransientAttribute is not null)
        {
            if (!model.IsDbDoc)
            {
                outputModel.Report(Diagnostics.Errors.TransientDbDoc, symbol);
            }
        }
        foreach (var methodModel in model.Methods)
        {
            Validate(outputModel, model, methodModel);
        }
        foreach (var propertyModel in model.Properties)
        {
            Validate(outputModel, model, propertyModel);
        }
    }
    static void Validate(OutputModel outputModel, ClassModel classModel, MethodModel model)
    {
    }
    static void Validate(OutputModel outputModel, ClassModel classModel, PropertyModel model)
    {
        var symbol = model.PropertySymbol;
        if (model.UseDefaultAttribute is not null)
        {
            if (symbol.IsStatic)
            {
                outputModel.Report(Diagnostics.Errors.UseDefaultStatic, symbol);
            }
            if (!classModel.IsDbDoc)
            {
                outputModel.Report(Diagnostics.Errors.UseDefaultDbDoc, symbol);
            }
            if (!outputModel.CanIncludeProperty(symbol))
            {
                outputModel.Report(Diagnostics.Warnings.UseDefaultIgnored, symbol);
            }
            if (symbol.Type.IsReferenceType && symbol.Type.NullableAnnotation != NullableAnnotation.Annotated)
            {
                outputModel.Report(Diagnostics.Errors.UseDefaultNullable, symbol);
            }
        }        
    }
}
