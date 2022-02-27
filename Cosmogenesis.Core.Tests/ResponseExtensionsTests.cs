using System.Net;
using Microsoft.Azure.Cosmos;
using Moq;

namespace Cosmogenesis.Core.Tests;

public class ResponseExtensionsTests
{
    readonly Mock<ResponseMessage> MockResponseMessage = new(MockBehavior.Strict);
    readonly Mock<TransactionalBatchResponse> MockTransactionalBatchResponse = new(MockBehavior.Strict);

    public ResponseExtensionsTests()
    {
        MockResponseMessage.Setup(x => x.StatusCode).Returns(HttpStatusCode.BadGateway).Verifiable();
        MockTransactionalBatchResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.BadGateway).Verifiable();
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void DbChangeFromErrorStatus_PreconditionFailed_ReturnsETagChanged() => Assert.Same(DbChange<DbDoc>.ETagChanged, HttpStatusCode.PreconditionFailed.DbChangeFromErrorStatus<DbDoc>());

    [Fact]
    [Trait("Type", "Unit")]
    public void DbChangeFromErrorStatus_Conflict_ReturnsAlreadyExists() => Assert.Same(DbChange<DbDoc>.AlreadyExists, HttpStatusCode.Conflict.DbChangeFromErrorStatus<DbDoc>());

    [Fact]
    [Trait("Type", "Unit")]
    public void DbChangeFromErrorStatus_NotFound_ReturnsMissing() => Assert.Same(DbChange<DbDoc>.Missing, HttpStatusCode.NotFound.DbChangeFromErrorStatus<DbDoc>());

    [Fact]
    [Trait("Type", "Unit")]
    public void DbChangeFromErrorStatus_429_Throws() => Assert.Throws<DbOverloadedException>(() => ((HttpStatusCode)429).DbChangeFromErrorStatus<DbDoc>());

    [Fact]
    [Trait("Type", "Unit")]
    public void DbChangeFromErrorStatusOther_Throws() => Assert.Throws<DbUnknownStatusCodeException>(() => HttpStatusCode.BadGateway.DbChangeFromErrorStatus<DbDoc>());

    [Fact]
    [Trait("Type", "Unit")]
    public void CreateResultFromErrorStatus_Conflict_ReturnsAlreadyExists() => Assert.Same(CreateResult<DbDoc>.AlreadyExists, HttpStatusCode.Conflict.CreateResultFromErrorStatus<DbDoc>());

    [Fact]
    [Trait("Type", "Unit")]
    public void CreateResultFromErrorStatus_429_Throws() => Assert.Throws<DbOverloadedException>(() => ((HttpStatusCode)429).CreateResultFromErrorStatus<DbDoc>());

    [Fact]
    [Trait("Type", "Unit")]
    public void CreateResultFromErrorStatusOther_Throws() => Assert.Throws<DbUnknownStatusCodeException>(() => HttpStatusCode.BadGateway.CreateResultFromErrorStatus<DbDoc>());

    [Fact]
    [Trait("Type", "Unit")]
    public void ReplaceResultFromErrorStatus_PreconditionFailed_ReturnsETagChanged() => Assert.Same(ReplaceResult<DbDoc>.ETagChanged, HttpStatusCode.PreconditionFailed.ReplaceResultFromErrorStatus<DbDoc>());

    [Fact]
    [Trait("Type", "Unit")]
    public void ReplaceResultFromErrorStatus_NotFound_ReturnsMissing() => Assert.Same(ReplaceResult<DbDoc>.Missing, HttpStatusCode.NotFound.ReplaceResultFromErrorStatus<DbDoc>());

    [Fact]
    [Trait("Type", "Unit")]
    public void ReplaceResultFromErrorStatus_429_Throws() => Assert.Throws<DbOverloadedException>(() => ((HttpStatusCode)429).ReplaceResultFromErrorStatus<DbDoc>());

    [Fact]
    [Trait("Type", "Unit")]
    public void ReplaceResultFromErrorStatusOther_Throws() => Assert.Throws<DbUnknownStatusCodeException>(() => HttpStatusCode.BadGateway.ReplaceResultFromErrorStatus<DbDoc>());

    [Fact]
    [Trait("Type", "Unit")]
    public void DeleteConflictTypeFromErrorStatus_PreconditionFailed_ReturnsETagChanged() => Assert.Equal(DbConflictType.ETagChanged, HttpStatusCode.PreconditionFailed.DeleteConflictTypeFromErrorStatus());

    [Fact]
    [Trait("Type", "Unit")]
    public void DeleteConflictTypeFromErrorStatus_NotFound_ReturnsMissing() => Assert.Equal(DbConflictType.Missing, HttpStatusCode.NotFound.DeleteConflictTypeFromErrorStatus());

    [Fact]
    [Trait("Type", "Unit")]
    public void DeleteConflictTypeFromErrorStatus_429_Throws() => Assert.Throws<DbOverloadedException>(() => ((HttpStatusCode)429).DeleteConflictTypeFromErrorStatus());

    [Fact]
    [Trait("Type", "Unit")]
    public void DeleteConflictTypeFromErrorStatusOther_Throws() => Assert.Throws<DbUnknownStatusCodeException>(() => HttpStatusCode.BadGateway.DeleteConflictTypeFromErrorStatus());

    [Fact]
    [Trait("Type", "Unit")]
    public void BatchResultFromErrorStatus_PreconditionFailed_ReturnsETagChanged() => Assert.Equal(BatchResult.ETagChanged, HttpStatusCode.PreconditionFailed.BatchResultFromErrorStatus());

    [Fact]
    [Trait("Type", "Unit")]
    public void BatchResultFromErrorStatus_NotFound_ReturnsMissing() => Assert.Equal(BatchResult.Missing, HttpStatusCode.NotFound.BatchResultFromErrorStatus());

    [Fact]
    [Trait("Type", "Unit")]
    public void BatchResultFromErrorStatus_Conflict_ReturnsAlreadyExists() => Assert.Equal(BatchResult.AlreadyExists, HttpStatusCode.Conflict.BatchResultFromErrorStatus());

    [Fact]
    [Trait("Type", "Unit")]
    public void BatchResultFromErrorStatus_429_Throws() => Assert.Throws<DbOverloadedException>(() => ((HttpStatusCode)429).BatchResultFromErrorStatus());

    [Fact]
    [Trait("Type", "Unit")]
    public void BatchResultFromErrorStatusOther_Throws() => Assert.Throws<DbUnknownStatusCodeException>(() => HttpStatusCode.BadGateway.BatchResultFromErrorStatus());

    [Fact]
    [Trait("Type", "Unit")]
    public void DbChangeFromErrorStatus_ResponseMessage_Passthru()
    {
        try
        {
            MockResponseMessage.Object.DbChangeFromErrorStatus<DbDoc>();
            Assert.False(true);
        }
        catch (DbUnknownStatusCodeException e)
        {
            Assert.Equal(HttpStatusCode.BadGateway, e.StatusCode);
        }
        MockResponseMessage.Verify();
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void CreateResultFromErrorStatus_ResponseMessage_Passthru()
    {
        try
        {
            MockResponseMessage.Object.CreateResultFromErrorStatus<DbDoc>();
            Assert.False(true);
        }
        catch (DbUnknownStatusCodeException e)
        {
            Assert.Equal(HttpStatusCode.BadGateway, e.StatusCode);
        }
        MockResponseMessage.Verify();
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ReplaceResultFromErrorStatus_ResponseMessage_Passthru()
    {
        try
        {
            MockResponseMessage.Object.ReplaceResultFromErrorStatus<DbDoc>();
            Assert.False(true);
        }
        catch (DbUnknownStatusCodeException e)
        {
            Assert.Equal(HttpStatusCode.BadGateway, e.StatusCode);
        }
        MockResponseMessage.Verify();
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void DeleteConflictTypeFromErrorStatus_ResponseMessage_Passthru()
    {
        try
        {
            MockResponseMessage.Object.DeleteConflictTypeFromErrorStatus();
            Assert.False(true);
        }
        catch (DbUnknownStatusCodeException e)
        {
            Assert.Equal(HttpStatusCode.BadGateway, e.StatusCode);
        }
        MockResponseMessage.Verify();
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void BatchResultFromErrorStatus_ResponseMessage_Passthru()
    {
        try
        {
            MockTransactionalBatchResponse.Object.BatchResultFromErrorStatus();
            Assert.False(true);
        }
        catch (DbUnknownStatusCodeException e)
        {
            Assert.Equal(HttpStatusCode.BadGateway, e.StatusCode);
        }
        MockTransactionalBatchResponse.Verify();
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ExceptionFromErrorStatus_ResponseMessage_Passthru()
    {
        var e = MockResponseMessage.Object.ExceptionFromErrorStatus() as DbUnknownStatusCodeException;
        Assert.Equal(HttpStatusCode.BadGateway, e!.StatusCode);
        MockResponseMessage.Verify();
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ExceptionFromErrorStatus_429_DbOverloadedException()
    {
        var e = ((HttpStatusCode)429).ExceptionFromErrorStatus();
        Assert.True(e is DbOverloadedException);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void ExceptionFromErrorStatus_Other_DbUnknownStatusCodeException()
    {
        var e = HttpStatusCode.BadGateway.ExceptionFromErrorStatus() as DbUnknownStatusCodeException;
        Assert.Equal(HttpStatusCode.BadGateway, e!.StatusCode);
    }
}
