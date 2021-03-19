using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Moq;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class ChangeFeedProcessorBaseTests
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public class TestChangeFeed : ChangeFeedProcessorBase
        {
            public TestChangeFeed(Container databaseContainer, Container leaseContainer, string processorName, int maxItemsPerBatch, TimeSpan? pollInterval, DateTime? startTime, Func<CancellationToken, Task> handleNewChangeFeedBatch, Func<CancellationToken, Task> handleFinishingBatch) : base(databaseContainer, leaseContainer, processorName, maxItemsPerBatch, pollInterval, startTime, handleNewChangeFeedBatch, handleFinishingBatch)
            {
            }

            protected sealed override Task? GetHandlerTask(DbDoc change, CancellationToken cancellationToken) => MockGetHandlerTask(change, cancellationToken);
            public virtual Task? MockGetHandlerTask(DbDoc change, CancellationToken cancellationToken) => throw new NotImplementedException();
            protected sealed override ChangeFeedProcessor CreateChangeFeedProcessor() => MockCreateChangeFeedProcessor();
            public virtual ChangeFeedProcessor MockCreateChangeFeedProcessor() => throw new NotImplementedException();

            protected sealed override Task ChangesHandler(IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken) => MockChangesHandler(changes, cancellationToken);
            protected sealed override Task HandleAllAtOnce(IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken) => MockHandleAllAtOnce(changes, cancellationToken);
            protected sealed override Task HandleSequential(IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken) => MockHandleSequential(changes, cancellationToken);
            protected sealed override Task HandleSequentialByPartition(IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken) => MockHandleSequentialByPartition(changes, cancellationToken);

            public virtual Task MockChangesHandler(IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken) => base.ChangesHandler(changes, cancellationToken);
            public virtual Task MockHandleAllAtOnce(IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken) => base.HandleAllAtOnce(changes, cancellationToken);
            public virtual Task MockHandleSequential(IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken) => base.HandleSequential(changes, cancellationToken);
            public virtual Task MockHandleSequentialByPartition(IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken) => base.HandleSequentialByPartition(changes, cancellationToken);

            protected sealed override TimeSpan PollInterval => base.PollInterval;
            protected sealed override Func<CancellationToken, Task> HandleFinishingBatch => base.HandleFinishingBatch;
            protected sealed override Func<CancellationToken, Task> HandleNewChangeFeedBatch => base.HandleNewChangeFeedBatch;
            protected sealed override ChangeFeedProcessor ChangeFeedProcessor => base.ChangeFeedProcessor;
            protected sealed override int MaxItemsPerBatch => base.MaxItemsPerBatch;
            protected sealed override DateTime StartTime => base.StartTime;
        }

        readonly Mock<Container> MockDatabaseContainer = new Mock<Container>(MockBehavior.Strict);
        readonly Mock<Container> MockLeaseContainer = new Mock<Container>(MockBehavior.Strict);
        readonly Mock<ChangeFeedProcessor> MockProcessor = new Mock<ChangeFeedProcessor>(MockBehavior.Strict);

        [Fact]
        [Trait("Type", "Unit")]
        public void DefaultPollInterval_BetweenMinAndMax() => Assert.True(ChangeFeedProcessorBase.DefaultPollInterval >= ChangeFeedProcessorBase.MinPollInterval && ChangeFeedProcessorBase.DefaultPollInterval <= ChangeFeedProcessorBase.MaxPollInterval);

        [Fact]
        [Trait("Type", "Unit")]
        public void MinPollInterval_NotNegative() => Assert.True(ChangeFeedProcessorBase.MinPollInterval >= TimeSpan.Zero);

        [Fact]
        [Trait("Type", "Unit")]
        public void Ctor_NullDatabaseContainer_Throws() => Assert.Throws<ArgumentNullException>(() => new TestChangeFeed(null!, MockLeaseContainer.Object, "asdf", 5, null, null, x => Task.CompletedTask, x => Task.CompletedTask));

        [Fact]
        [Trait("Type", "Unit")]
        public void Ctor_NullLeaseContainer_Throws() => Assert.Throws<ArgumentNullException>(() => new TestChangeFeed(MockDatabaseContainer.Object, null!, "asdf", 5, null, null, x => Task.CompletedTask, x => Task.CompletedTask));

        [Fact]
        [Trait("Type", "Unit")]
        public void Ctor_NullProcessorName_Throws() => Assert.Throws<ArgumentNullException>(() => new TestChangeFeed(MockDatabaseContainer.Object, MockLeaseContainer.Object, null!, 5, null, null, x => Task.CompletedTask, x => Task.CompletedTask));

        [Fact]
        [Trait("Type", "Unit")]
        public void Ctor_ZeroMaxItems_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => new TestChangeFeed(MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 0, null, null, x => Task.CompletedTask, x => Task.CompletedTask));

        [Fact]
        [Trait("Type", "Unit")]
        public void Ctor_NegativePollInterval_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => new TestChangeFeed(MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 5, TimeSpan.FromSeconds(-1), null, x => Task.CompletedTask, x => Task.CompletedTask));

        [Fact]
        [Trait("Type", "Unit")]
        public void Ctor_NullNewChangeFeedBatch_Throws() => Assert.Throws<ArgumentNullException>(() => new TestChangeFeed(MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 5, null, null, null!, x => Task.CompletedTask));

        [Fact]
        [Trait("Type", "Unit")]
        public void Ctor_NullFinishingBatch_Throws() => Assert.Throws<ArgumentNullException>(() => new TestChangeFeed(MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 5, null, null, x => Task.CompletedTask, null!));

        readonly Func<CancellationToken, Task> Cancel = _ => Task.CompletedTask;

        [Fact]
        [Trait("Type", "Unit")]
        public void StartAsync_CallsStartAsync()
        {
            MockProcessor.Setup(x => x.StartAsync()).Returns(Task.CompletedTask).Verifiable();
            var feed = new Mock<TestChangeFeed>(MockBehavior.Strict, MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 5, null, null, Cancel, Cancel);
            feed.Setup(x => x.MockCreateChangeFeedProcessor()).Returns(MockProcessor.Object).Verifiable();
            feed.Setup(x => x.StartAsync()).CallBase();

            Assert.Equal(Task.CompletedTask, feed.Object.StartAsync());

            MockProcessor.Verify();
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void StopAsync_CallsStopAsync()
        {
            MockProcessor.Setup(x => x.StopAsync()).Returns(Task.CompletedTask).Verifiable();
            var feed = new Mock<TestChangeFeed>(MockBehavior.Strict, MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 5, null, null, Cancel, Cancel);
            feed.Setup(x => x.MockCreateChangeFeedProcessor()).Returns(MockProcessor.Object).Verifiable();
            feed.Setup(x => x.StopAsync()).CallBase();

            Assert.Equal(Task.CompletedTask, feed.Object.StopAsync());

            MockProcessor.Verify();
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ChangesHandler_NullChanges_ReturnsSynchronously()
        {
            var feed = new Mock<TestChangeFeed>(MockBehavior.Strict, MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 5, null, null, Cancel, Cancel);
            feed.Setup(x => x.MockChangesHandler(null!, It.IsAny<CancellationToken>())).CallBase();
            var result = feed.Object.MockChangesHandler(null!, default);

            Assert.True(result.IsCompletedSuccessfully);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ChangesHandler_EmptyChanges_ReturnsSynchronously()
        {
            var feed = new Mock<TestChangeFeed>(MockBehavior.Strict, MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 5, null, null, Cancel, Cancel);
            feed.Setup(x => x.MockChangesHandler(Array.Empty<DbDoc>(), It.IsAny<CancellationToken>())).CallBase();
            var result = feed.Object.MockChangesHandler(Array.Empty<DbDoc>(), default);

            Assert.True(result.IsCompletedSuccessfully);
        }

        bool HandleNewChangeFeedBatchCalled;
        bool HandleFinishingBatchCalled;
        bool HandleCalled;

        Task HandleNew(CancellationToken cancellationToken)
        {
            Assert.False(HandleNewChangeFeedBatchCalled);
            Assert.False(HandleFinishingBatchCalled);
            Assert.False(HandleCalled);
            HandleNewChangeFeedBatchCalled = true;
            return Task.CompletedTask;
        }
        Task HandleFinish(CancellationToken cancellationToken)
        {
            Assert.True(HandleNewChangeFeedBatchCalled);
            Assert.False(HandleFinishingBatchCalled);
            Assert.True(HandleCalled);
            HandleFinishingBatchCalled = true;
            return Task.CompletedTask;
        }
        Task HandleDocs(IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken)
        {
            Assert.True(HandleNewChangeFeedBatchCalled);
            Assert.False(HandleFinishingBatchCalled);
            Assert.False(HandleCalled);
            HandleCalled = true;
            return Task.CompletedTask;
        }

        readonly TestDoc[] ChangedDocs = new[]
        {
            new TestDoc{ pk = "a"},
            new TestDoc{ pk = "b"},
            new TestDoc{ pk = "a"},
        };

        [Fact]
        [Trait("Type", "Unit")]
        public async Task ChangesHandler_Changes_AllAtOnce()
        {
            var feed = new Mock<TestChangeFeed>(MockBehavior.Strict, MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 5, null, null, (Func<CancellationToken, Task>)HandleNew, (Func<CancellationToken, Task>)HandleFinish);
            feed.Setup(x => x.MockChangesHandler(ChangedDocs, It.IsAny<CancellationToken>())).CallBase();
            feed.Setup(x => x.ProcessingMode).Returns(ChangeFeedProcessingMode.AllAtOnce).Verifiable();
            feed
                .Setup(x => x.MockHandleAllAtOnce(ChangedDocs, It.IsAny<CancellationToken>()))
                .Returns((IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken) => HandleDocs(changes, cancellationToken))
                .Verifiable();

            await feed.Object.MockChangesHandler(ChangedDocs, default);

            feed.Verify();
            Assert.True(HandleNewChangeFeedBatchCalled);
            Assert.True(HandleFinishingBatchCalled);
            Assert.True(HandleCalled);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public async Task ChangesHandler_Changes_SequentialByPartition()
        {
            var feed = new Mock<TestChangeFeed>(MockBehavior.Strict, MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 5, null, null, (Func<CancellationToken, Task>)HandleNew, (Func<CancellationToken, Task>)HandleFinish);
            feed.Setup(x => x.MockChangesHandler(ChangedDocs, It.IsAny<CancellationToken>())).CallBase();
            feed.Setup(x => x.ProcessingMode).Returns(ChangeFeedProcessingMode.SequentialByPartition).Verifiable();
            feed
                .Setup(x => x.MockHandleSequentialByPartition(ChangedDocs, It.IsAny<CancellationToken>()))
                .Returns((IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken) => HandleDocs(changes, cancellationToken))
                .Verifiable();

            await feed.Object.MockChangesHandler(ChangedDocs, default);

            feed.Verify();
            Assert.True(HandleNewChangeFeedBatchCalled);
            Assert.True(HandleFinishingBatchCalled);
            Assert.True(HandleCalled);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public async Task ChangesHandler_Changes_Sequential()
        {
            var feed = new Mock<TestChangeFeed>(MockBehavior.Strict, MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 5, null, null, (Func<CancellationToken, Task>)HandleNew, (Func<CancellationToken, Task>)HandleFinish);
            feed.Setup(x => x.MockChangesHandler(ChangedDocs, It.IsAny<CancellationToken>())).CallBase();
            feed.Setup(x => x.ProcessingMode).Returns(ChangeFeedProcessingMode.Sequential).Verifiable();
            feed
                .Setup(x => x.MockHandleSequential(ChangedDocs, It.IsAny<CancellationToken>()))
                .Returns((IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken) => HandleDocs(changes, cancellationToken))
                .Verifiable();

            await feed.Object.MockChangesHandler(ChangedDocs, default);

            feed.Verify();
            Assert.True(HandleNewChangeFeedBatchCalled);
            Assert.True(HandleFinishingBatchCalled);
            Assert.True(HandleCalled);
        }

        readonly TaskCompletionSource<bool> TaskCompletionSource = new TaskCompletionSource<bool>();

        [Fact]
        [Trait("Type", "Unit")]
        public async Task HandleAllAtOnce()
        {
            var called = new List<DbDoc>();
            var feed = new Mock<TestChangeFeed>(MockBehavior.Strict, MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 5, null, null, (Func<CancellationToken, Task>)HandleNew, (Func<CancellationToken, Task>)HandleFinish);
            var explode = Task.Delay(5000).ContinueWith(_ => { if (!TaskCompletionSource.Task.IsCompleted) { throw new Exception(); } });
            feed
                .Setup(x => x.MockGetHandlerTask(It.IsAny<DbDoc>(), It.IsAny<CancellationToken>()))
                .Returns((DbDoc doc, CancellationToken c) =>
                {
                    lock (called)
                    {
                        called.Add(doc);
                        if (called.Count == ChangedDocs.Length)
                        {
                            TaskCompletionSource.SetResult(true);
                        }
                    }
                    return Task.WhenAny(explode, TaskCompletionSource.Task);
                })
                .Verifiable();
            feed.Setup(x => x.MockHandleAllAtOnce(ChangedDocs, It.IsAny<CancellationToken>())).CallBase();

            await feed.Object.MockHandleAllAtOnce(ChangedDocs, default);

            feed.Verify();
        }

        [Fact]
        [Trait("Type", "Unit")]
        public async Task HandleSequential()
        {
            var feed = new Mock<TestChangeFeed>(MockBehavior.Strict, MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 5, null, null, (Func<CancellationToken, Task>)HandleNew, (Func<CancellationToken, Task>)HandleFinish);
            var explode = Task.Delay(5000).ContinueWith(_ => { if (!TaskCompletionSource.Task.IsCompleted) { throw new Exception(); } });
            var previousTask = Task.CompletedTask;
            feed
                .Setup(x => x.MockGetHandlerTask(It.IsAny<DbDoc>(), It.IsAny<CancellationToken>()))
                .Returns((DbDoc doc, CancellationToken c) =>
                {
                    Assert.True(previousTask.IsCompletedSuccessfully);
                    return previousTask = Task.Delay(10);
                })
                .Verifiable();
            feed.Setup(x => x.MockHandleSequential(ChangedDocs, It.IsAny<CancellationToken>())).CallBase();

            await feed.Object.MockHandleSequential(ChangedDocs, default);

            feed.Verify();
        }

        [Fact]
        [Trait("Type", "Unit")]
        public async Task HandleSequentialByPartition()
        {
            var feed = new Mock<TestChangeFeed>(MockBehavior.Strict, MockDatabaseContainer.Object, MockLeaseContainer.Object, "asdf", 5, null, null, (Func<CancellationToken, Task>)HandleNew, (Func<CancellationToken, Task>)HandleFinish);
            var explode = Task.Delay(5000).ContinueWith(_ => { if (!TaskCompletionSource.Task.IsCompleted) { throw new Exception(); } });
            var previousTasksByPk = new Dictionary<string, List<Task>>();
            feed
                .Setup(x => x.MockGetHandlerTask(It.IsAny<DbDoc>(), It.IsAny<CancellationToken>()))
                .Returns((DbDoc doc, CancellationToken c) =>
                {
                    lock (previousTasksByPk)
                    {
                        var prev = Task.Delay(10);
                        if (previousTasksByPk.TryGetValue(doc.pk, out var previousTasks))
                        {
                            Assert.True(previousTasks.All(x => x.IsCompletedSuccessfully));
                            previousTasks.Add(prev);
                        }
                        else
                        {
                            previousTasksByPk[doc.pk] = new List<Task> { prev };
                        }
                        return prev;
                    }
                })
                .Verifiable();
            feed.Setup(x => x.MockHandleSequentialByPartition(ChangedDocs, It.IsAny<CancellationToken>())).CallBase();

            await feed.Object.MockHandleSequentialByPartition(ChangedDocs, default);

            feed.Verify();
        }
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
