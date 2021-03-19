using System;
using System.Net;

namespace Cosmogenesis.Core
{
    public static class DbModelFactory
    {
        public static BatchResult CreateBatchResult(DbConflictType conflict) => new BatchResult(conflict: conflict);
        public static BatchResult CreateBatchResult() => new BatchResult(0);

        public static CreateResult<T> CreateCreateResult<T>(DbConflictType conflict) where T : DbDoc =>
            new CreateResult<T>(conflict: conflict);
        public static CreateResult<T> CreateCreateResult<T>(T document) where T : DbDoc =>
            new CreateResult<T>(document: document);

        public static ReplaceResult<T> CreateReplaceResult<T>(DbConflictType conflict) where T : DbDoc =>
            new ReplaceResult<T>(conflict: conflict);
        public static ReplaceResult<T> CreateReplaceResult<T>(T document) where T : DbDoc =>
            new ReplaceResult<T>(document: document);

        public static CreateOrReplaceResult<T> CreateCreateOrReplaceResult<T>(T document, bool alreadyExisted) where T : DbDoc =>
            new CreateOrReplaceResult<T>(document: document, alreadyExisted: alreadyExisted);

        public static ReadOrCreateResult<T> CreateReadOrCreateResult<T>(T document, bool alreadyExisted) where T : DbDoc =>
            new ReadOrCreateResult<T>(document: document, alreadyExisted: alreadyExisted);

        public static DbUnknownStatusCodeException CreateDbUnknownStatusCodeException(HttpStatusCode statusCode) => new DbUnknownStatusCodeException(statusCode: statusCode);
        public static DbOverloadedException CreateDbOverloadedException() => new DbOverloadedException();
        public static DbOverloadedException CreateDbOverloadedException(string message) => new DbOverloadedException(message: message);
        public static DbOverloadedException CreateDbOverloadedException(string message, Exception innerException) => new DbOverloadedException(message: message, innerException: innerException);
        public static DbConflictException CreateDbConflictException(DbConflictType dbConflictType) => new DbConflictException(dbConflictType: dbConflictType);
    }
}
