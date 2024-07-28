namespace mem0.Core;

public enum Distance
{
    /// <summary>
    /// 未知距离
    /// </summary>
    UnknownDistance = 0,

    /// <summary>
    /// 余弦相似度
    /// </summary>
    Cosine = 1,

    /// <summary>
    /// 欧几里得距离
    /// </summary>
    Euclid = 2,

    /// <summary>
    /// 点积
    /// </summary>
    Dot = 3,

    /// <summary>
    /// 曼哈顿距离
    /// </summary>
    Manhattan = 4,
}