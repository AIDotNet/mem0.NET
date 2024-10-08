﻿namespace mem0.NET.Options;

/// <summary>
/// mem0 配置
/// </summary>
public class Mem0Options
{
    /// <summary>
    /// OpenAI Endpoint https://api.token-ai.cn
    /// </summary>
    public string OpenAIEndpoint { get; set; }

    /// <summary>
    /// OpenAI Key
    /// </summary>
    public string OpenAIKey { get; set; }

    /// <summary>
    /// 对话模型
    /// </summary>
    public string OpenAIChatCompletionModel { get; set; }
    
    /// <summary>
    /// 文本嵌入模型
    /// </summary>
    public string OpenAITextEmbeddingModel { get; set; }

    /// <summary>
    /// Collection Name
    /// </summary>
    public string CollectionName { get; set; }

    /// <summary>
    /// Vector size
    /// </summary>
    public ulong VectorSize { get; set; } = 1536;
}