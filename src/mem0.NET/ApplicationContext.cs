using mem0.NET.Service.DataAccess;

namespace mem0.Core;

public class ApplicationContext
{
    /// <summary>
    /// 用于存储当前上下文的对象
    /// </summary>
    public static AsyncLocal<Dictionary<string, string>> Current { get; } =
        new();

    private static AsyncLocal<Mem0DbContext> dbContext { get; } =
        new();

    public static Mem0DbContext DbContext
    {
        get => dbContext.Value;
        set => dbContext.Value = value;
    }
}