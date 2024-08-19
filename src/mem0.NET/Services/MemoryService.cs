using System.Text.Encodings.Web;
using System.Text.Json;
using mem0.Core;
using mem0.Core.Model;
using mem0.Core.VectorStores;
using mem0.NET.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;

#pragma warning disable SKEXP0050

#pragma warning disable SKEXP0001

namespace mem0.NET.Services;

/// <summary>
/// Memory service.
/// </summary>
public class MemoryService(
    IServiceProvider serviceProvider,
    IVectorStoreService vectorStoreService,
    ITextEmbeddingGenerationService textEmbeddingGenerationService,
    IChatCompletionService chatCompletionService,
    IOptions<Mem0Options> options,
    IHistoryService historyService)
{
    private const string MemoryDeductionPrompt = """
                                                 Deduce the facts, preferences, and memories from the provided text.
                                                 Just return the facts, preferences, and memories in bullet points:
                                                 Natural language text: {user_input}
                                                 User/Agent details: {metadata}

                                                 Constraint for deducing facts, preferences, and memories:
                                                 - The facts, preferences, and memories should be concise and informative.
                                                 - Don't start by "The person likes Pizza". Instead, start with "Likes Pizza".
                                                 - Don't remember the user/agent details provided. Only remember the facts, preferences, and memories.
                                                 - Output content in the original language

                                                 Deduced facts, preferences, and memories:
                                                 """;

    private const string UpdateMemoryPrompt = """
                                              You are an expert at merging, updating, and organizing memories. When provided with existing memories and new information, your task is to merge and update the memory list to reflect the most accurate and current information. You are also provided with the matching score for each existing memory to the new information. Make sure to leverage this information to make informed decisions about which memories to update or merge.

                                              Guidelines:
                                              - Eliminate duplicate memories and merge related memories to ensure a concise and updated list.
                                              - If a memory is directly contradicted by new information, critically evaluate both pieces of information:
                                                  - If the new memory provides a more recent or accurate update, replace the old memory with new one.
                                                  - If the new memory seems inaccurate or less detailed, retain the original and discard the old one.
                                              - Maintain a consistent and clear style throughout all memories, ensuring each entry is concise yet informative.
                                              - If the new memory is a variation or extension of an existing memory, update the existing memory to reflect the new information.

                                              Here are the details of the task:
                                              - Existing Memories:
                                              {existing_memories}

                                              - New Memory: {memory}
                                              """;


    /// <summary>
    /// 创建记忆
    /// </summary>
    /// <param name="input"></param>
    public async Task CreateMemoryAsync(CreateMemoryInput input)
    {
        var embeddings = await textEmbeddingGenerationService.GenerateEmbeddingAsync(input.Data);

        var filters = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(input.UserId))
        {
            filters.Add("user_id", input.UserId);
        }

        if (!string.IsNullOrEmpty(input.AgentId))
        {
            filters.Add("agent_id", input.AgentId);
        }

        if (!string.IsNullOrEmpty(input.RunId))
        {
            filters.Add("run_id", input.RunId);
        }

        var chatHistory = new ChatHistory();

        if (string.IsNullOrEmpty(input.Prompt))
        {
            input.Prompt = MemoryDeductionPrompt.Replace("{user_input}", input.Data)
                .Replace("{metadata}", JsonSerializer.Serialize(input.MetaData));
        }

        var extracted_memories = await chatCompletionService.GetChatMessageContentAsync(new ChatHistory()
        {
            new(AuthorRole.System,
                "You are an expert at deducing facts, preferences and memories from unstructured text."),
            new(AuthorRole.User, input.Prompt)
        });

        await vectorStoreService.CreateColAsync(options.Value.CollectionName, options.Value.VectorSize);

        var existing_memories = (
                await vectorStoreService.SearchAsync(options.Value.CollectionName, embeddings.ToArray(), 5, filters))
            .Select(x => new
            {
                id = x.Id,
                score = x.Score,
                metaData = x.Payload,
                text = x.Text
            });

        var serialized_existing_memories = existing_memories.Select(x => new
        {
            memoryId = x.id,
            score = x.score,
            text = x.text
        });

        var messages = get_update_memory_messages(serialized_existing_memories, extracted_memories.Content);

        chatHistory.AddRange(messages);

        using var scope = serviceProvider.CreateScope();
        var kernel = scope.ServiceProvider.GetService<Kernel>();

        var content = await chatCompletionService.GetChatMessageContentAsync(chatHistory,
            new OpenAIPromptExecutionSettings()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            }, kernel);
    }

    private List<ChatMessageContent> get_update_memory_messages(object serializedExistingMemories,
        string existingMemories)
    {
        return new List<ChatMessageContent>()
        {
            new(AuthorRole.User,
                UpdateMemoryPrompt.Replace("{existing_memories}",
                        JsonSerializer.Serialize(serializedExistingMemories, new JsonSerializerOptions()
                        {
                            // utf8 encoding
                            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        }))
                    .Replace("{memory}", existingMemories))
        };
    }

    /// <summary>
    /// 创建记忆工具
    /// </summary>
    /// <param name="input"></param>
    public async Task CreateMemoryToolAsync(CreateMemoryToolInput input)
    {
        var embeddings = await textEmbeddingGenerationService.GenerateEmbeddingAsync(input.Data);

        var memoryId = Guid.NewGuid();
        input.MetaData.Add("data", input.Data);
        input.MetaData.Add("created_at", DateTime.Now);

        await vectorStoreService.GetAsync(options.Value.CollectionName, memoryId);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="memoryId"></param>
    /// <returns></returns>
    public async Task<VectorData> GetMemory(Guid memoryId)
    {
        var memory = await vectorStoreService.GetAsync(options.Value.CollectionName, memoryId);

        if (memory == null)
        {
            return new VectorData();
        }

        return memory;
    }

    /// <summary>
    /// 获取所有记忆
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="agentId"></param>
    /// <param name="runId"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public async Task<List<VectorData>> GetMemoryAll(string? userId,
        string? agentId, string? runId, uint limit = 100)
    {
        var filters = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(userId))
        {
            filters.Add("user_id", userId);
        }

        if (!string.IsNullOrEmpty(agentId))
        {
            filters.Add("agent_id", agentId);
        }

        if (!string.IsNullOrEmpty(runId))
        {
            filters.Add("run_id", runId);
        }

        var memories = await vectorStoreService.GetListAsync(options.Value.CollectionName, filters, limit);

        return memories;
    }

    /// <summary>
    /// 搜索记忆
    /// </summary>
    /// <param name="query"></param>
    /// <param name="userId"></param>
    /// <param name="agentId"></param>
    /// <param name="runId"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public async Task<List<VectorData>> SearchMemory(string query, string? userId,
        string? agentId, string? runId, uint limit = 100)
    {
        var embeddings = await textEmbeddingGenerationService.GenerateEmbeddingAsync(query);

        var filters = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(userId))
        {
            filters.Add("user_id", userId);
        }

        if (!string.IsNullOrEmpty(agentId))
        {
            filters.Add("agent_id", agentId);
        }

        if (!string.IsNullOrEmpty(runId))
        {
            filters.Add("run_id", runId);
        }

        var memories =
            await vectorStoreService.SearchAsync(options.Value.CollectionName, embeddings.ToArray(), limit, filters);

        return memories.Select(x => new VectorData()
        {
            Id = x.Id,
            Score = x.Score,
            MetaData = x.Payload.ToDictionary(y => y.Key, y => y.Value),
        }).ToList();
    }

    /// <summary>
    /// 更新记忆
    /// </summary>
    /// <param name="input"></param>
    public async Task Update(UpdateMemoryInput input)
    {
        using var scope = serviceProvider.CreateScope();
        var memoryToolService = scope.ServiceProvider.GetService<MemoryToolService>();

        await memoryToolService.UpdateMemoryAsync(input.MemoryId, input.Data);
    }

    /// <summary>
    /// 删除指定记忆
    /// </summary>
    /// <param name="memoryId"></param>
    public async Task Delete(Guid memoryId)
    {
        using var scope = serviceProvider.CreateScope();
        var memoryToolService = scope.ServiceProvider.GetService<MemoryToolService>();
        await memoryToolService.DeleteMemoryAsync(memoryId);
    }

    /// <summary>
    /// 删除所有记忆
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="agentId"></param>
    /// <param name="runId"></param>
    /// <exception cref="Exception"></exception>
    public async Task DeleteAll(string? userId, string? agentId, string? runId)
    {
        var filters = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(userId))
        {
            filters.Add("user_id", userId);
        }

        if (!string.IsNullOrEmpty(agentId))
        {
            filters.Add("agent_id", agentId);
        }

        if (!string.IsNullOrEmpty(runId))
        {
            filters.Add("run_id", runId);
        }

        if (filters.Count == 0)
        {
            throw new Exception(
                "At least one filter is required to delete all memories. If you want to delete all memories, use the `reset()` method.");
        }

        var memories = await vectorStoreService.GetListAsync(options.Value.CollectionName, filters);

        using var scope = serviceProvider.CreateScope();
        var memoryToolService = scope.ServiceProvider.GetService<MemoryToolService>();

        foreach (var memory in memories)
        {
            await memoryToolService.DeleteMemoryAsync(memory.Id);
        }
    }

    /// <summary>
    /// 获取历史
    /// </summary>
    /// <param name="memoryId"></param>
    /// <returns></returns>
    public async Task<PagingDto<History>> GetHistory(string memoryId, int page, int pageSize)
    {
        var histories = await historyService.GetHistoriesAsync(memoryId, page, pageSize);

        return histories;
    }

    /// <summary>
    /// 重置记忆
    /// </summary>
    public async Task Reset()
    {
        await vectorStoreService.DeleteColAsync(options.Value.CollectionName);

        await historyService.ResetHistoryAsync();
    }
}