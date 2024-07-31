namespace mem0.Core;

public enum VectorInfoStatus
{
    UnknownCollectionStatus = 0,

    /// <summary>
    /// 所有的片段都准备好了
    /// </summary>
    Green = 1,

    /// <summary>
    /// 过程优化
    /// </summary>
    Yellow = 2,

    /// <summary>
    /// 出现问题
    /// </summary>
    Red = 3,

    /// <summary>
    /// 优化正在等待
    /// </summary>
    Grey = 4,
}