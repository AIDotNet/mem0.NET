using mem0.Core;
using mem0.Core.VectorStores;
using mem0.NET.Functions;
using mem0.NET.Options;
using mem0.NET.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;

#pragma warning disable SKEXP0001

namespace mem0.NET.Service.Services;

public static class ServiceCollectionExtensions
{
    public static WebApplication MapMemoryService(this WebApplication app)
    {
        var memoryService = app.MapGroup("/api/v1/memory")
            .WithTags("Memory")
            .WithDescription("Memory management")
            .WithDisplayName("Memory");

        memoryService.MapPost("/memory", async (MemoryService memoryService,
            CreateMemoryInput input,
            Mem0DotNetOptions mem0DotNetOptions) =>
        {
            await memoryService.CreateMemoryAsync(input,
                mem0DotNetOptions);
        }).WithDescription("创建记忆").WithDisplayName("创建记忆").WithTags("记忆").WithName("CreateMemory");

        memoryService.MapPost("memory_tool", async (MemoryService memoryService,
            CreateMemoryToolInput input,
            Mem0DotNetOptions mem0DotNetOptions) =>
        {
            await memoryService.CreateMemoryToolAsync(input,
                mem0DotNetOptions);
        }).WithDescription("创建记忆工具").WithDisplayName("创建记忆工具").WithTags("记忆");

        memoryService.MapGet("history/{memoryId}",
                async (MemoryService memoryService, string memoryId) => await memoryService.GetHistory(memoryId))
            .WithDescription("获取历史").WithDisplayName("获取历史").WithTags("记忆");

        memoryService.MapGet("memory/{memoryId}", async (MemoryService memoryService, string memoryId,
                    Mem0DotNetOptions options) =>
                await memoryService.GetMemory(memoryId, options))
            .WithDescription("获取记忆")
            .WithDisplayName("获取记忆")
            .WithTags("记忆");

        memoryService.MapGet("memory", async (MemoryService memoryService,
                    Mem0DotNetOptions options,
                    string? userId,
                    string? agentId, string? runId, uint limit) =>
                await memoryService.GetMemoryAll(options, userId, agentId, runId, limit))
            .WithDescription("获取所有记忆")
            .WithDisplayName("获取所有记忆")
            .WithTags("记忆");

        memoryService.MapGet("search", async (MemoryService memoryService,
                    Mem0DotNetOptions options,
                    string query,
                    string? userId,
                    string? agentId, string? runId, uint limit) =>
                await memoryService.SearchMemory(options, query,
                    userId,
                    agentId, runId, limit))
            .WithDescription("搜索记忆")
            .WithDisplayName("搜索记忆")
            .WithTags("记忆");

        memoryService.MapPut("memory", async (MemoryService memoryService, UpdateMemoryInput input,
                MemoryTool memoryTool) => await memoryService.Update(input, memoryTool))
            .WithDescription("更新记忆")
            .WithDisplayName("更新记忆")
            .WithTags("记忆");

        memoryService.MapDelete("memory/{memoryId}", async (MemoryService memoryService, string memoryId,
                MemoryTool memoryTool) => await memoryService.Delete(memoryId, memoryTool))
            .WithDescription("删除记忆")
            .WithDisplayName("删除记忆")
            .WithTags("记忆");

        memoryService.MapDelete("memory", async (MemoryService memoryService, string? userId,
                    string? agentId, string? runId,
                    Mem0DotNetOptions options, MemoryTool memoryTool) =>
                await memoryService.DeleteAll(userId, agentId, runId, options, memoryTool))
            .WithDescription("删除所有记忆")
            .WithDisplayName("删除所有记忆")
            .WithTags("记忆");

        memoryService.MapDelete("reset", async (MemoryService memoryService, IVectorStoreService vectorStoreService,
                    Mem0DotNetOptions mem0DotNetOptions, IHistoryService historyService) =>
                await memoryService.Reset(vectorStoreService, mem0DotNetOptions, historyService))
            .WithDescription("重置记忆")
            .WithDisplayName("重置记忆")
            .WithTags("记忆");

        return app;
    }
}