using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using mem0.Core;
using mem0.Core.VectorStores;
using mem0.NET.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

#pragma warning disable SKEXP0001

namespace mem0.NET.Functions;

public class MemoryTool(
    IServiceProvider serviceProvider,
    ILogger<MemoryTool> logger)
{
    [KernelFunction, Description("add a memory")]
    public async Task AddMemory([Required] [Description("Data to add to memory")] string data)
    {
        var currentValue = Mem0Context.Current.Value;

        await using var scope = serviceProvider.CreateAsyncScope();
        var options = scope.ServiceProvider.GetRequiredService<Mem0DotNetOptions>();
        var vectorStoreService = scope.ServiceProvider.GetService<IVectorStoreService>();
        var historyService = scope.ServiceProvider.GetService<IHistoryService>();
        var textEmbeddingGenerationService = scope.ServiceProvider.GetService<ITextEmbeddingGenerationService>();
        var embeddings = await textEmbeddingGenerationService.GenerateEmbeddingAsync(data);
        var memoryId = Guid.NewGuid().ToString();
        var metadata = new Dictionary<string, object>
        {
            { "data", data },
            { "created_at", DateTime.Now },
            { "user_id", currentValue["userId"] },
            { "run_id", currentValue["runId"] }
        };
        await vectorStoreService.Insert(options.CollectionName,
            [[..embeddings.ToArray()]],
            [metadata], [memoryId]);

        await historyService.AddHistory(memoryId, string.Empty, data, "add");

        logger.LogInformation("添加缓存" + data + " memoryId:" + memoryId);

        await Task.CompletedTask;
    }

    [KernelFunction, Description("Update memory provided Id and data")]
    public async Task UpdateMemory([Required] [Description("memoryid of the memory to update")] string memoryId,
        [Required] [Description("Updated data for the memory")]
        string data)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var options = scope.ServiceProvider.GetRequiredService<Mem0DotNetOptions>();
        var vectorStoreService = scope.ServiceProvider.GetService<IVectorStoreService>();
        var historyService = scope.ServiceProvider.GetService<IHistoryService>();
        var existingMemory = await vectorStoreService.Get(options.CollectionName, memoryId);

        var prevValue = existingMemory.MetaData["data"];
        var newMetadata = new Dictionary<string, object>
        {
            { "data", prevValue },
            { "updated_at", DateTime.Now },
        };

        foreach (var o in existingMemory.MetaData)
        {
            if (o.Key != "data")
                newMetadata.Add(o.Key, o.Value);
        }

        await vectorStoreService.Update(options.CollectionName, memoryId,
            [..existingMemory.Vector.ToArray()], newMetadata);

        await historyService.AddHistory(memoryId, prevValue.ToString(), data, "update");

        logger.LogInformation("更新缓存 memoryId:" + memoryId + " data:" + data);
    }


    [KernelFunction, Description("Delete memory by memory_id")]
    public async Task DeleteMemory([Required] [Description("memoryid of the memory to update")] string memoryId)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var options = scope.ServiceProvider.GetRequiredService<Mem0DotNetOptions>();
        var vectorStoreService = scope.ServiceProvider.GetService<IVectorStoreService>();
        var historyService = scope.ServiceProvider.GetService<IHistoryService>();
        var existingMemory = await vectorStoreService.Get(options.CollectionName, memoryId);

        var prevValue = existingMemory.MetaData["data"];

        await vectorStoreService.Delete(options.CollectionName, memoryId);

        logger.LogInformation("删除缓存 memoryId:" + memoryId);

        await historyService.AddHistory(memoryId, prevValue.ToString(), string.Empty, "delete", true);
    }
}