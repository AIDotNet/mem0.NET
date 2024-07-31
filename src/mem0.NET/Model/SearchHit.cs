namespace mem0.Core.Model;

public class SearchHit
{
    public object Id { get; set; } // 向量 ID
    public float Score { get; set; } // 相似度分数
    public Dictionary<string, object> Payload { get; set; } // 有效载荷

    public Dictionary<string, object> MetaData { get; set; }

    public string? Text
    {
        get
        {
            if (Payload.TryGetValue("data", out var data))
            {
                return data.ToString();
            }

            return string.Empty;
        }
    }
}