using System.Net;

namespace Cosmogenesis.Core;

public static class DbModelFactory
{
    public static BatchResult CreateBatchResult(DbConflictType conflict) => new(conflict: conflict);
    public static BatchResult CreateBatchResult() => new(0);

    public static CreateResult<T> CreateCreateResult<T>(DbConflictType conflict) where T : DbDoc =>
        new(conflict: conflict);
    public static CreateResult<T> CreateCreateResult<T>(T document) where T : DbDoc =>
        new(document: document);

    public static ReplaceResult<T> CreateReplaceResult<T>(DbConflictType conflict) where T : DbDoc =>
        new(conflict: conflict);
    public static ReplaceResult<T> CreateReplaceResult<T>(T document) where T : DbDoc =>
        new(document: document);

    public static CreateOrReplaceResult<T> CreateCreateOrReplaceResult<T>(T document, bool alreadyExisted) where T : DbDoc =>
        new(document: document, alreadyExisted: alreadyExisted);

    public static ReadOrCreateResult<T> CreateReadOrCreateResult<T>(T document, bool alreadyExisted) where T : DbDoc =>
        new(document: document, alreadyExisted: alreadyExisted);

    public static DbUnknownStatusCodeException CreateDbUnknownStatusCodeException(HttpStatusCode statusCode) => new(statusCode: statusCode);
    public static DbOverloadedException CreateDbOverloadedException() => new();
    public static DbOverloadedException CreateDbOverloadedException(string message) => new(message: message);
    public static DbOverloadedException CreateDbOverloadedException(string message, Exception innerException) => new(message: message, innerException: innerException);
    public static DbConflictException CreateDbConflictException(DbConflictType dbConflictType) => new(dbConflictType: dbConflictType);
}
