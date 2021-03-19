namespace Cosmogenesis.Generator.Writers
{
    static class DocumentModelWriter
    {
        public static string CreateAndCheckPkAndId(DbDocumentModel documentModel, string paramTypeName) => $@"
            if ({paramTypeName} is null)
            {{
                throw new ArgumentNullException(nameof({paramTypeName}));
            }}
            var calculatedPk = {documentModel.DbPartitionModel.GetKeyModel.FullMethodName}({documentModel.DbPartitionModel.GetKeyModel.DocumentToParametersMapping(paramTypeName)});
            var calculatedId = DbDocHelper.GetValidId({documentModel.GetIdModel.FullMethodName}({documentModel.GetIdModel.DocumentToParametersMapping(paramTypeName)}));
            if ({paramTypeName}.id is null)
            {{
                {paramTypeName}.id = calculatedId ?? throw new InvalidOperationException(""The generated document id cannot be null"");
            }}
            else if ({paramTypeName}.id != calculatedId) 
            {{
                throw new InvalidOperationException(""The document .id property does not match the calculated document id"");
            }}
            if ({paramTypeName}.pk is null)
            {{
                {paramTypeName}.pk = calculatedPk ?? throw new InvalidOperationException(""The generated partition key cannot be null"");;
            }}
            else if ({paramTypeName}.pk != calculatedPk)
            {{
                throw new InvalidOperationException(""The document .pk property does not match the calculated document partition key"");
            }}
";
    }
}
