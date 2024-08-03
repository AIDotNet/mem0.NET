using mem0.Core;
using mem0.Core.VectorStores;
using mem0.NET.Functions;
using mem0.NET.Options;
using mem0.NET.Services;

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
            CreateMemoryInput input) =>
        {
            await memoryService.CreateMemoryAsync(input);
        }).WithDescription("创建记忆").WithDisplayName("创建记忆").WithTags("记忆").WithName("CreateMemory");

        memoryService.MapPost("memory_tool", async (MemoryService memoryService,
            CreateMemoryToolInput input) =>
        {
            await memoryService.CreateMemoryToolAsync(input);
        }).WithDescription("创建记忆工具").WithDisplayName("创建记忆工具").WithTags("记忆");

        memoryService.MapGet("history/{memoryId}",
                async (MemoryService memoryService, string memoryId, int page, int pageSize) =>
                    await memoryService.GetHistory(memoryId, page, pageSize))
            .WithDescription("获取历史").WithDisplayName("获取历史").WithTags("记忆");

        memoryService.MapGet("memory/{memoryId}", async (MemoryService memoryService, Guid memoryId) =>
                await memoryService.GetMemory(memoryId))
            .WithDescription("获取记忆")
            .WithDisplayName("获取记忆")
            .WithTags("记忆");

        memoryService.MapGet("memory", async (MemoryService memoryService,
                    string? userId,
                    string? agentId, string? runId, uint limit) =>
                await memoryService.GetMemoryAll(userId, agentId, runId, limit))
            .WithDescription("获取所有记忆")
            .WithDisplayName("获取所有记忆")
            .WithTags("记忆");

        memoryService.MapGet("search", async (MemoryService memoryService,
                    string query,
                    string? userId,
                    string? agentId, string? runId, uint limit) =>
                await memoryService.SearchMemory(query,
                    userId,
                    agentId, runId, limit))
            .WithDescription("搜索记忆")
            .WithDisplayName("搜索记忆")
            .WithTags("记忆");

        memoryService.MapPut("memory",
                async (MemoryService memoryService, UpdateMemoryInput input) => await memoryService.Update(input))
            .WithDescription("更新记忆")
            .WithDisplayName("更新记忆")
            .WithTags("记忆");

        memoryService.MapDelete("memory/{memoryId}",
                async (MemoryService memoryService, Guid memoryId) => await memoryService.Delete(memoryId))
            .WithDescription("删除记忆")
            .WithDisplayName("删除记忆")
            .WithTags("记忆");

        memoryService.MapDelete("memory", async (MemoryService memoryService, string? userId,
                    string? agentId, string? runId) =>
                await memoryService.DeleteAll(userId, agentId, runId))
            .WithDescription("删除所有记忆")
            .WithDisplayName("删除所有记忆")
            .WithTags("记忆");

        memoryService.MapDelete("reset", async (MemoryService memoryService) =>
                await memoryService.Reset())
            .WithDescription("重置记忆")
            .WithDisplayName("重置记忆")
            .WithTags("记忆");

        return app;
    }
}