namespace mem0.Core;

public class Mem0Context
{
    /// <summary>
    /// 用于存储当前上下文的对象
    /// </summary>
    public static AsyncLocal<Dictionary<string, string>> Current { get; } =
        new();
}