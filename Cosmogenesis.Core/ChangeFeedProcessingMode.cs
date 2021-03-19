namespace Cosmogenesis.Core
{
    public enum ChangeFeedProcessingMode
    {
        /// <summary>
        /// Handler tasks are started one-by-one chronologically, but then run concurrently starting at the first 'await' in the handler.
        /// </summary>
        AllAtOnce = 1,

        /// <summary>
        /// Documents are split into groups by their partition key.
        /// In each partition, handler tasks are completed one-by-one chronologically, without any concurrency.
        /// Separate partitions process their tasks concurrently.
        /// </summary>
        SequentialByPartition = 2,

        /// <summary>
        /// Handler tasks are completed one-by-one chronologically, without any concurrency.
        /// </summary>
        Sequential = 3
    }
}
