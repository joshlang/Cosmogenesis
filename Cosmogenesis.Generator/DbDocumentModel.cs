using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator
{
    class DbDocumentModel
    {
        public readonly DbPartitionModel DbPartitionModel;
        public readonly string Name;
        public readonly INamedTypeSymbol TypeSymbol;
        public readonly Dictionary<string, DbPropertyModel> Properties = new(StringComparer.OrdinalIgnoreCase);
        public readonly bool IsTransient, IsMutable;
        public readonly DbMethodModel GetIdModel;

        public readonly string ClassName, ClassFullName;
        public readonly string ConstDocType;
        public readonly string TypeId;

        public string PropertiesAsSetters => string.Join(", ", Properties.Values.Select(x => $"{x.PropertySymbol.Name} = {x.PropertySymbol.Name.Parameterify()}"));
        public string PropertiesAsInputParameters => string.Join(", ", Properties.Values.OrderBy(x => x.UseDefault).Select(x => $"{x.PropertySymbol.Type.FullTypeName()}{(x.NullableReferenceType ? "?" : "")} {x.PropertySymbol.Name.Parameterify()}{(x.UseDefault ? " = default" : "")}"));

        public DbDocumentModel(DbPartitionModel dbPartitionModel, string name, string typeId, INamedTypeSymbol typeSymbol, bool isTransient, bool isMutable, DbMethodModel getIdModel)
        {
            DbPartitionModel = dbPartitionModel;
            Name = name;
            TypeSymbol = typeSymbol;
            IsTransient = isTransient;
            IsMutable = isMutable;
            GetIdModel = getIdModel;
            TypeId = typeId;

            ClassName = typeSymbol.Name;
            ClassFullName = typeSymbol.FullTypeName();
            ConstDocType = $"{DbPartitionModel.DbModel.TypesClassName}.{dbPartitionModel.ClassName}.{ClassName}";
        }
    }
}
