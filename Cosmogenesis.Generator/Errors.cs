using Microsoft.CodeAnalysis;

namespace Cosmogenesis.Generator
{
    static class Errors
    {
        public static DiagnosticDescriptor DbParameters = new DiagnosticDescriptor("DBE001", "DbAttribute", "DbAttribute parameters are wonky", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor GeneratedName = new DiagnosticDescriptor("DBE002", "Naming", "The name generated for this item is not a valid identifier", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor DbPartitionParameters = new DiagnosticDescriptor("DBE003", "DbPartitionAttribute", "DbPartitionAttribute parameters are wonky", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor DbPartitionClassNotStatic = new DiagnosticDescriptor("DBE004", "DbPartitionAttribute", "DbPartitionAttribute must be attached to a static class or static method", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor DbPartitionClassWithName = new DiagnosticDescriptor("DBE005", "DbPartitionAttribute", "DbPartitionAttribute must not provide a name when attached to a static class", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor UnknownDbName = new DiagnosticDescriptor("DBE006", "DbName", "Could not determine the database name; attach a DbAttribute", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor DbPartitionRedeclared = new DiagnosticDescriptor("DBE00&", "DbName", "Partition is declared more than once", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor DbPartitionMethodNotStatic = new DiagnosticDescriptor("DBE008", "DbPartitionAttribute", "DbPartitionAttribute method must be static", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor DbPartitionMethodNotReturningString = new DiagnosticDescriptor("DBE009", "DbPartitionAttribute", "DbPartitionAttribute method must return a string", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor DbPartitionDefinitionMethodNotAccessible = new DiagnosticDescriptor("DBE010", "DbPartitionDefinitionAttribute", "DbPartitionDefinitionAttribute method must be public or internal", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor UnknownPartitionName = new DiagnosticDescriptor("DBE011", "PartitionName", "Could not determine the partition name; attach a DbPartitionAttribute", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor DuplicateDocumentName = new DiagnosticDescriptor("DBE012", "DocumentName", "Two documents cannot have the same name in the same partition", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor DuplicatePropertyName = new DiagnosticDescriptor("DBE013", "DocumentName", "Two properties cannot have the same name in the same document (case insensitive)", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor PartitionName = new DiagnosticDescriptor("DBE014", "DbPartitionDefinitionAttribute", "Partition name is not a valid identifier", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor MissingDocumentId = new DiagnosticDescriptor("DBE015", "DbDocumentIdAttribute", $"Missing document id generator method; attach a DbDocumentIdAttribute or define a {Types.GetIdDefaultName} method", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor DbNamespace = new DiagnosticDescriptor("DBE016", "DbAttribute", "DbAttribute parameters specify different namespaces for the same database", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor UpperCase = new DiagnosticDescriptor("DBE017", "Naming", "Objects and properties of the database should begin with an uppercase character", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor LowerCase = new DiagnosticDescriptor("DBE017", "Naming", "Methods generating partition keys and ids should contain parameters that begin with a lowercase character", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor MissingProperty = new DiagnosticDescriptor("DBE018", "Naming", "Parameter name should have a matching document property with an uppercase first letter and accessible getter and setter", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor PropertyTypeMismatch = new DiagnosticDescriptor("DBE019", "Naming", "Parameter and matching document property should be of the same type (including nullability)", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor UndefinedPartition = new DiagnosticDescriptor("DBE020", "Partition", "Partition not defined; attach a PartitionDefinitionAttribute", "Db", DiagnosticSeverity.Error, true);
        public static DiagnosticDescriptor DuplicateTypeId = new DiagnosticDescriptor("DBE021", "TypeId", "Two documents cannot have the same TypeId in the same database", "Db", DiagnosticSeverity.Error, true);
    }

    static class Warnings
    {
        public static DiagnosticDescriptor DbPartitionClassNotReturningString = new DiagnosticDescriptor("DBW001", "DbPartitionAttribute", "Public/Internal method in a class marked with DbPartitionAttribute does not return a string and will be ignored", "Db", DiagnosticSeverity.Warning, true);
        public static DiagnosticDescriptor DbDocNotPublic = new DiagnosticDescriptor("DBW002", "DbDoc", "DbDoc was discovered but is not public, so it will be ignored", "Db", DiagnosticSeverity.Warning, true);
        
    }
}
