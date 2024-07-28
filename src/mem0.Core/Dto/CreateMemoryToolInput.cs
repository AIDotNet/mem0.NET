namespace mem0.Core;

public class CreateMemoryToolInput
{
    public string Data { get; set; }

    public Dictionary<string,object> MetaData { get; set; } = new();
}