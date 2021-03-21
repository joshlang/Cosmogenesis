using System.Collections.Generic;

namespace Cosmogenesis.Generator
{
    class DbPartitionModel
    {
        public readonly DbModel DbModel;
        public readonly string Name;
        public readonly Dictionary<string, DbDocumentModel> Documents = new();
        public readonly DbMethodModel GetKeyModel;

        public readonly string ClassName;
        public readonly string QueryBuilderClassName;
        public readonly string QueryClassName;
        public readonly string BatchClassName;
        public readonly string ReadClassName;
        public readonly string ReadOrThrowClassName;
        public readonly string ReadManyClassName;
        public readonly string CreateClassName;
        public readonly string CreateOrReplaceClassName;
        public readonly string ReadOrCreateClassName;
        public readonly string ChangeFeedHandlersClassName;

        public DbPartitionModel(DbModel dbModel, string name, DbMethodModel getKeyModel)
        {
            DbModel = dbModel;
            Name = name;
            GetKeyModel = getKeyModel;

            ClassName = name.AddSuffix(Suffixes.Partition);
            QueryBuilderClassName = name.AddSuffix(Suffixes.QueryBuilder);
            QueryClassName = name.AddSuffix(Suffixes.Query);
            BatchClassName = name.AddSuffix(Suffixes.Batch);
            ReadClassName = name.AddSuffix(Suffixes.Read);
            ReadOrThrowClassName = name.AddSuffix(Suffixes.ReadOrThrow);
            ReadManyClassName = name.AddSuffix(Suffixes.ReadMany);
            CreateClassName = name.AddSuffix(Suffixes.Create);
            ReadOrCreateClassName = name.AddSuffix(Suffixes.ReadOrCreate);
            CreateOrReplaceClassName = name.AddSuffix(Suffixes.CreateOrReplace);
            ChangeFeedHandlersClassName = name.AddSuffix(Suffixes.ChangeFeedHandlers);
        }
    }
}
