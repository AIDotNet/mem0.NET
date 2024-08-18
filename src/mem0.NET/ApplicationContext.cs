namespace mem0.Core;

public class ApplicationContext
{
    /// <summary>
    /// 当前上下文需要的添加内存元数据
    /// </summary>
    public static AsyncLocal<Dictionary<string, string>> AddMemoryMetadata { get; } =
        new();

    /// <summary>
    /// 当前上下文需要写入的历史记录Id，如果为空则默认生成
    /// </summary>
    public static AsyncLocal<string> HistoryTrackId { get; set; } = new();
    
    /// <summary>
    /// 当前上下文需要写入的历史记录用户Id
    /// </summary>
    public static AsyncLocal<string> HistoryUserId { get; set; } = new();
}