using mem0.Core;
using mem0.Core.Model;
using mem0.Core.VectorStores;
using mem0.NET.Functions;
using mem0.NET.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;

#pragma warning disable SKEXP0001

namespace mem0.NET.Services;

public interface IMemoryService
{
    Task CreateMemoryAsync(CreateMemoryInput input,
        ITextEmbeddingGenerationService textEmbeddingGenerationService,
        IChatCompletionService chatCompletionService,
        Kernel kernel,
        Mem0DotNetOptions mem0DotNetOptions);

    Task CreateMemoryToolAsync(CreateMemoryToolInput input,
        ITextEmbeddingGenerationService textEmbeddingGenerationService,
        IChatCompletionService chatCompletionService,
        Mem0DotNetOptions mem0DotNetOptions);

    Task<VectorData> GetMemory(string memoryId,
        Mem0DotNetOptions options);

    Task<List<VectorData>> GetMemoryAll(Mem0DotNetOptions options,
        string? userId,
        string? agentId, string? runId, uint limit = 100);

    Task<List<VectorData>> SearchMemory(Mem0DotNetOptions options,
        ITextEmbeddingGenerationService textEmbeddingGenerationService,
        IChatCompletionService chatCompletionService,
        string query, string? userId,
        string? agentId, string? runId, uint limit = 100);

    Task Update(UpdateMemoryInput input, MemoryTool memoryTool);

    Task Delete(string memoryId, MemoryTool memoryTool);

    Task DeleteAll(string? userId, string? agentId, string? runId,
        Mem0DotNetOptions options, MemoryTool memoryTool);

    Task<List<History>> GetHistory(string memoryId, IHistoryService historyService);

    Task Reset(Mem0DotNetOptions mem0DotNetOptions);
}