using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator
{
    class DbPropertyModel
    {
        public readonly DbDocumentModel DocumentModel;
        public readonly string Name;
        public readonly IPropertySymbol PropertySymbol;

        public readonly bool NullableReferenceType;
        public readonly bool UseDefault;

        public DbPropertyModel(DbDocumentModel documentModel, string name, IPropertySymbol propertySymbol, bool useDefault)
        {
            DocumentModel = documentModel;
            Name = name;
            PropertySymbol = propertySymbol;
            UseDefault = useDefault;

            NullableReferenceType = propertySymbol.Type.IsReferenceType && propertySymbol.Type.NullableAnnotation == NullableAnnotation.Annotated;
        }
    }
}
