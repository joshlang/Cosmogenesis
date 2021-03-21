using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cosmogenesis.Generator
{
    class DbModel
    {
        public readonly string Name;
        public readonly Dictionary<string, DbPartitionModel> Partitions = new();
        public readonly AttributeSyntax Syntax;

        public readonly string Namespace;
        public readonly string DbClassName;
        public readonly string DbPartitionsClassName;
        public readonly string SerializerClassName;
        public readonly string ConverterClassName;
        public readonly string ReadClassName;
        public readonly string QueryClassName;
        public readonly string QueryBuilderClassName;
        public readonly string TypesClassName;
        public readonly string DbFactoryClassName;
        public readonly string ChangeFeedHandlersClassName;
        public readonly string ChangeFeedProcessorClassName;

        public DbModel(string name, AttributeSyntax syntax, string @namespace)
        {
            Name = name;
            Syntax = syntax;

            Namespace = @namespace;
            DbClassName = name.AddSuffix(Suffixes.Database);
            DbPartitionsClassName = name.AddSuffix(Suffixes.Partitions);
            SerializerClassName = name.AddSuffix(Suffixes.Serializer);
            ConverterClassName = name.AddSuffix(Suffixes.Converter);
            ReadClassName = name.AddSuffix(Suffixes.Read);
            QueryClassName = name.AddSuffix(Suffixes.Query);
            QueryBuilderClassName = name.AddSuffix(Suffixes.QueryBuilder);
            TypesClassName = name.AddSuffix(Suffixes.Types);
            DbFactoryClassName = name.AddSuffix(Suffixes.DatabaseFactory);
            ChangeFeedHandlersClassName = name.AddSuffix(Suffixes.ChangeFeedHandlers);
            ChangeFeedProcessorClassName = name.AddSuffix(Suffixes.ChangeFeedProcessor);
        }
    }
}
