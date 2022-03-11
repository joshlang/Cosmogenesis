namespace Cosmogenesis.Core;

public enum BatchProcessingMode
{
    /// <summary>
    /// All documents are processed concurrently.
    /// </summary>
    AllAtOnce = 1,

    /// <summary>
    /// Documents are split into groups by their partition key.
    /// In each partition, documents are processed one-by-one chronologically, without any concurrency.
    /// All partitions process their tasks concurrently.
    /// </summary>
    SequentialByPartition = 2,

    /// <summary>
    /// Documents are processed one-by-one chronologically, without any concurrency.
    /// </summary>
    Sequential = 3
}
