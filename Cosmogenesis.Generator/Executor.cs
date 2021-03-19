using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cosmogenesis.Generator.Writers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cosmogenesis.Generator
{
    class Executor
    {
        readonly GeneratorExecutionContext Context;
        readonly CosmosSyntaxReceiver Syntax;

        readonly Dictionary<string, DbModel> Databases = new();

        bool CanGenerate = true;

        public Executor(GeneratorExecutionContext context, CosmosSyntaxReceiver syntax)
        {
            Context = context;
            Syntax = syntax ?? throw new ArgumentNullException(nameof(syntax));
        }

        static Location? GetLocation(object? syntaxNodeOrSymbol) =>
            syntaxNodeOrSymbol is SyntaxNode node
            ? node.GetLocation()
            : syntaxNodeOrSymbol is ISymbol symbol
            ? symbol.Locations.FirstOrDefault()
            : null;
        void Error(DiagnosticDescriptor descriptor, object? syntaxNodeOrSymbol)
        {
            CanGenerate = false;
            Context.ReportDiagnostic(Diagnostic.Create(descriptor, GetLocation(syntaxNodeOrSymbol)));
        }
        void Warning(DiagnosticDescriptor descriptor, object? syntaxNodeOrSymbol) => Context.ReportDiagnostic(Diagnostic.Create(descriptor, GetLocation(syntaxNodeOrSymbol)));

        public void Generate()
        {
            BuildDbModel();
            if (!CanGenerate)
            {
                return;
            }
            foreach (var db in Databases.Values)
            {
                DbWriter.Write(Context, db);
            }
        }

        void BuildDbModel()
        {
            FindDatabases();
            FindPartitionsInClasses();
            FindPartitionMethods();
            FindDocuments();
            ValidateNames();
        }

        void ValidateName(object? syntaxNodeOrSymbol, params string[] names)
        {
            foreach (var name in names)
            {
                if (string.IsNullOrEmpty(name) || !SyntaxFacts.IsValidIdentifier(name))
                {
                    Error(Errors.GeneratedName, syntaxNodeOrSymbol);
                }
                else if (!char.IsUpper(name[0]))
                {
                    Error(Errors.UpperCase, syntaxNodeOrSymbol);
                }
            }
        }
        void ValidateParameters(IMethodSymbol methodSymbol)
        {
            foreach (var p in methodSymbol.Parameters)
            {
                if (string.IsNullOrEmpty(p.Name) || !char.IsLower(p.Name[0]))
                {
                    Error(Errors.LowerCase, methodSymbol);
                }
            }
        }
        void ValidateMatchingParameters(DbDocumentModel documentModel)
        {
            foreach (var p in documentModel.GetIdModel.MethodSymbol.Parameters.Concat(documentModel.DbPartitionModel.GetKeyModel.MethodSymbol.Parameters))
            {
                if (!documentModel.Properties.TryGetValue(p.Name, out var prop) ||
                    p.Name != prop.Name.Parameterify())
                {
                    Error(Errors.MissingProperty, p);
                }
                else if (!p.Type.Equals(prop.PropertySymbol.Type, SymbolEqualityComparer.IncludeNullability))
                {
                    Error(Errors.PropertyTypeMismatch, p);
                }
            }
        }

        void ValidateNames()
        {
            foreach (var db in Databases.Values)
            {
                ValidateName(db.Syntax, db.Namespace.Split('.'));
                ValidateName(db.Syntax,
                    db.DbClassName,
                    db.DbPartitionsClassName,
                    db.Name,
                    db.SerializerClassName,
                    db.ConverterClassName,
                    db.ReadClassName,
                    db.QueryClassName,
                    db.QueryBuilderClassName,
                    db.TypesClassName);
                foreach (var partition in db.Partitions.Values)
                {
                    ValidateName(partition.GetKeyModel.MethodSymbol,
                        partition.ClassName,
                        partition.QueryBuilderClassName,
                        partition.QueryClassName,
                        partition.BatchClassName,
                        partition.ReadClassName,
                        partition.ReadOrThrowClassName,
                        partition.ReadManyClassName,
                        partition.CreateClassName,
                        partition.CreateOrReplaceClassName,
                        partition.ReadOrCreateClassName);
                    ValidateParameters(partition.GetKeyModel.MethodSymbol);
                    foreach (var doc in partition.Documents.Values)
                    {
                        ValidateName(doc.TypeSymbol,
                            doc.ClassName);
                        ValidateParameters(doc.GetIdModel.MethodSymbol);
                        ValidateMatchingParameters(doc);
                        foreach (var prop in doc.Properties.Values)
                        {
                            ValidateName(prop.PropertySymbol,
                                prop.Name);
                        }
                    }
                }
            }
        }

        void FindDatabases()
        {
            List<(string DbName, string? Namespace, AttributeSyntax Syntax)> attributes = new();
            foreach (var a in Syntax.DbAttributes)
            {
                var model = Context.Compilation.GetSemanticModel(a.SyntaxTree);
                var args = a.ArgumentList?.Arguments;
                if (args is null ||
                    args.Value.Count < 1 ||
                    args.Value[0].NameEquals is not null ||
                    model.GetConstantValue(args.Value[0].Expression).Value?.ToString() is not string dbName)
                {
                    Error(Errors.DbParameters, a);
                    continue;
                }
                var ns = args
                    .Value
                    .Where(x => x.NameEquals?.Name.Identifier.Text == Types.DbNamespaceParameter)
                    .Select(x => model.GetConstantValue(x.Expression).Value?.ToString())
                    .FirstOrDefault();
                attributes.Add((dbName, ns, a));
            }
            foreach (var byName in attributes.GroupBy(x => x.DbName))
            {
                var dbnamespace = byName.Select(x => x.Namespace).ExcludeNull().Distinct().DefaultIfEmpty("Cosmogenesis.Generated").ToList();
                var syntax = byName.First().Syntax;
                if (dbnamespace.Count > 1)
                {
                    Error(Errors.DbNamespace, syntax);
                    continue;
                }
                Databases[byName.Key] = new DbModel(name: byName.Key, syntax: syntax, @namespace: dbnamespace[0]);
            }
        }

        static AttributeData? FindAttribute(ISymbol? symbol, string attributeType, bool includeContainer)
        {
            var a = symbol?.GetAttributes().SingleOrDefault(x => x.AttributeClass?.Name == attributeType);
            if (a is not null || symbol is null)
            {
                return a;
            }
            if (symbol is ITypeSymbol typeSymbol)
            {
                a = FindAttribute(typeSymbol.BaseType, attributeType, includeContainer);
                if (a is not null)
                {
                    return a;
                }
            }
            if (!includeContainer || symbol.ContainingType is not INamedTypeSymbol containedSymbol)
            {
                return null;
            }
            return FindAttribute(containedSymbol, attributeType, true);
        }
        static string? NameFromAttribute(ISymbol? symbol, string attributeType, bool includeContainer) => // assumes name is first constructor parameter
            FindAttribute(symbol, attributeType, includeContainer)?.ConstructorArguments.FirstOrDefault().Value?.ToString();
        static string? DbNameFromAttribute(ISymbol? symbol, bool includeContainer) => NameFromAttribute(symbol, Types.DbAttribute, includeContainer);
        static string? PartitionNameFromAttribute(ISymbol? symbol, bool includeContainer) => NameFromAttribute(symbol, Types.PartitionAttribute, includeContainer);

        DbModel? GetDbModelFromAttributeOrDefault(ISymbol symbol)
        {
            var dbName = DbNameFromAttribute(symbol, true);
            if (dbName != null && Databases.TryGetValue(dbName, out var db))
            {
                return db;
            }
            var partitionName = PartitionNameFromAttribute(symbol, true);
            if (partitionName != null)
            {
                var dbs = Databases.Values.Where(x => x.Partitions.Any(p => p.Value.Name == partitionName)).ToList();
                if (dbs.Count == 1)
                {
                    return dbs[0];
                }
            }
            if (Databases.Count == 1)
            {
                return Databases.Values.First();
            }
            Error(Errors.UnknownDbName, symbol);
            return null;
        }

        DbPartitionModel? GetDbPartitionModelFromAttributeOrDefault(ISymbol symbol)
        {
            var dbModel = GetDbModelFromAttributeOrDefault(symbol);
            if (dbModel is null)
            {
                return null;
            }
            var partitionName = PartitionNameFromAttribute(symbol, true);
            if (partitionName != null)
            {
                if (dbModel.Partitions.TryGetValue(partitionName, out var part))
                {
                    return part;
                }

                Error(Errors.UndefinedPartition, symbol);
                return null;
            }
            if (dbModel.Partitions.Count == 1)
            {
                return dbModel.Partitions.Values.First();
            }
            Error(Errors.UnknownPartitionName, symbol);
            return null;
        }

        void FindPartitionsInClasses()
        {
            foreach (var c in Syntax.PartitionDefinitionClasses)
            {
                var model = Context.Compilation.GetSemanticModel(c.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(c);
                if (symbol is null ||
                    symbol.TypeKind != TypeKind.Class ||
                    !symbol.IsStatic)
                {
                    Error(Errors.DbPartitionClassNotStatic, c);
                    continue;
                }
                var partitionAttribute = symbol.GetAttributes().SingleOrDefault(x => x.AttributeClass?.Name == Types.PartitionDefinitionAttribute);
                if (partitionAttribute is null)
                {
                    continue;
                }
                if (partitionAttribute.ConstructorArguments.Length > 0)
                {
                    Error(Errors.DbPartitionClassWithName, c);
                    continue;
                }
                if (GetDbModelFromAttributeOrDefault(symbol) is not DbModel dbModel)
                {
                    continue;
                }
                foreach (var method in symbol
                    .GetMembers()
                    .Select(x => x as IMethodSymbol)
                    .ExcludeNull()
                    .Where(x => x.MethodKind == MethodKind.Ordinary)
                    .Where(x => x.DeclaredAccessibility.IsAccessible()))
                {
                    if (method.ReturnType.SpecialType != SpecialType.System_String)
                    {
                        Warning(Warnings.DbPartitionClassNotReturningString, method);
                        continue;
                    }
                    AddPartition(dbModel, method);
                }
            }
        }

        void FindPartitionMethods()
        {
            foreach (var m in Syntax.PartitionDefinitionMethods)
            {
                var model = Context.Compilation.GetSemanticModel(m.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(m);
                if (symbol is null ||
                    symbol.MethodKind != MethodKind.Ordinary ||
                    !symbol.IsStatic)
                {
                    Error(Errors.DbPartitionMethodNotStatic, m);
                    continue;
                }
                if (!symbol.DeclaredAccessibility.IsAccessible())
                {
                    Error(Errors.DbPartitionDefinitionMethodNotAccessible, m);
                    continue;
                }
                if (symbol.ReturnType.SpecialType != SpecialType.System_String)
                {
                    Error(Errors.DbPartitionMethodNotReturningString, m);
                    continue;
                }
                if (GetDbModelFromAttributeOrDefault(symbol) is not DbModel dbModel)
                {
                    continue;
                }
                AddPartition(dbModel, symbol);
            }
        }

        void AddPartition(DbModel dbModel, IMethodSymbol getPartitionKeyMethod)
        {
            var partitionAttribute = getPartitionKeyMethod
                .GetAttributes()
                .FirstOrDefault(x => x.AttributeClass?.Name == Types.PartitionDefinitionAttribute && x.ConstructorArguments.Length == 1);
            var name = partitionAttribute?.ConstructorArguments[0].Value?.ToString() ?? getPartitionKeyMethod.Name;
            if (dbModel.Partitions.TryGetValue(name, out var partitionModel) &&
                !partitionModel.GetKeyModel.MethodSymbol.Equals(getPartitionKeyMethod, SymbolEqualityComparer.Default))
            {
                Error(Errors.DbPartitionRedeclared, getPartitionKeyMethod);
                return;
            }
            var getKeyModel = new DbMethodModel(getPartitionKeyMethod);
            dbModel.Partitions[name] = new DbPartitionModel(dbModel, name, getKeyModel);
        }

        void FindDocuments()
        {
            var dbDoc = Context.Compilation.GetTypeByMetadataName(Types.FullDbDoc);
            bool IsDbDoc(INamedTypeSymbol? symbol)
            {
                return
                    symbol is null
                    ? false
                    : symbol.Equals(dbDoc, SymbolEqualityComparer.Default)
                    ? true
                    : IsDbDoc(symbol.BaseType);
            }
            static IMethodSymbol? FindGetIdByAttribute(INamedTypeSymbol symbol)
            {
                var method = symbol
                    .GetMembers()
                    .Select(x => x as IMethodSymbol)
                    .ExcludeNull()
                    .Where(x => x.IsStatic && x.ReturnType.SpecialType == SpecialType.System_String)
                    .FirstOrDefault(x => x.GetAttributes().Any(a => a.AttributeClass?.Name == Types.DocumentIdAttribute));
                if (method is not null || symbol.BaseType is null)
                {
                    return method;
                }
                return FindGetIdByAttribute(symbol.BaseType);
            }
            static IMethodSymbol? FindGetIdByConvention(INamedTypeSymbol symbol)
            {
                var method = symbol
                    .GetMembers()
                    .Select(x => x as IMethodSymbol)
                    .ExcludeNull()
                    .Where(x => x.IsStatic && x.ReturnType.SpecialType == SpecialType.System_String)
                    .FirstOrDefault(x => x.Name == Types.GetIdDefaultName);
                if (method is not null || symbol.BaseType is null)
                {
                    return method;
                }
                return FindGetIdByConvention(symbol.BaseType);
            }

            foreach (var c in Syntax.SubClasses)
            {
                var model = Context.Compilation.GetSemanticModel(c.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(c);
                if (symbol is null ||
                    symbol.IsAbstract ||
                    symbol.TypeKind != TypeKind.Class ||
                    !IsDbDoc(symbol))
                {
                    continue;
                }

                if (symbol.DeclaredAccessibility != Accessibility.Public)
                {
                    Warning(Warnings.DbDocNotPublic, symbol);
                    continue;
                }

                if (GetDbPartitionModelFromAttributeOrDefault(symbol) is not DbPartitionModel partitionModel)
                {
                    continue;
                }

                var getIdMethod = FindGetIdByAttribute(symbol) ?? FindGetIdByConvention(symbol);
                if (getIdMethod is null)
                {
                    Error(Errors.MissingDocumentId, symbol);
                    continue;
                }
                var getIdModel = new DbMethodModel(getIdMethod);

                var name = symbol.Name;
                if (partitionModel.Documents.ContainsKey(name))
                {
                    Error(Errors.DuplicateDocumentName, symbol);
                    continue;
                }
                var isTransient = FindAttribute(symbol, Types.TransientAttribute, false) is not null;
                var isMutable = FindAttribute(symbol, Types.MutableAttribute, false) is not null;

                var doc = new DbDocumentModel(partitionModel, name, symbol, isTransient: isTransient, isMutable: isMutable, getIdModel: getIdModel);
                partitionModel.Documents[name] = doc;
                FindDocumentMembers(doc);
            }
        }

        void FindDocumentMembers(DbDocumentModel documentModel)
        {
            var typeSymbol = documentModel.TypeSymbol;
            while (typeSymbol != null && typeSymbol.FullTypeName() != Types.FullDbDoc)
            {
                foreach (var propertySymbol in typeSymbol
                    .GetMembers()
                    .Select(x => x as IPropertySymbol)
                    .ExcludeNull()
                    .Where(x => !x.IsStatic && x.SetMethod is not null && x.GetMethod is not null)
                    .Where(x => !x.GetAttributes().Any(a => a.AttributeClass?.Name == Types.JsonIgnoreAttribute))
                    .Where(x => x.DeclaredAccessibility.IsAccessible())
                    .Where(x => x.SetMethod!.DeclaredAccessibility.IsAccessible())
                    .Where(x => x.GetMethod!.DeclaredAccessibility.IsAccessible()))
                {
                    var name = propertySymbol.Name;
                    if (documentModel.Properties.ContainsKey(name))
                    {
                        Error(Errors.DuplicatePropertyName, propertySymbol);
                        continue;
                    }

                    var useDefault = FindAttribute(propertySymbol, Types.UseDefaultAttribute, false) is not null;
                    documentModel.Properties[name] = new DbPropertyModel(documentModel, name, propertySymbol, useDefault: useDefault);
                }
                typeSymbol = typeSymbol.BaseType;
            }
        }
    }
}
