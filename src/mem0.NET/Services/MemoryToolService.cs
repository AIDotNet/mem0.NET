using mem0.Core;
using mem0.Core.VectorStores;
using mem0.NET.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Embeddings;

#pragma warning disable SKEXP0001

namespace mem0.NET.Services;

public class MemoryToolService(
    ILogger<MemoryToolService> logger,
    ITextEmbeddingGenerationService textEmbeddingGenerationService,
    IHistoryService historyService,
    IVectorStoreService vectorStoreService,
    IOptions<Mem0Options> options)
{
    public async Task AddMemory(string data)
    {
        var currentValue = ApplicationContext.Current.Value;

        var embeddings = await textEmbeddingGenerationService.GenerateEmbeddingAsync(data);
        var memoryId = Guid.NewGuid().ToString();
        var metadata = new Dictionary<string, object>
        {
            { "data", data },
            { "created_at", DateTime.Now }
        };

        if (currentValue != null)
        {
            foreach (var item in currentValue)
            {
                metadata.Add(item.Key, item.Value);
            }
        }

        await vectorStoreService.Insert(options.Value.CollectionName,
            [[..embeddings.ToArray()]],
            [metadata], [memoryId]);

        await historyService.AddHistory(memoryId, string.Empty, data, "add");

        logger.LogInformation("添加缓存" + data + " memoryId:" + memoryId);

        await Task.CompletedTask;
    }

    public async Task UpdateMemory(string memoryId,
        string data)
    {
        var existingMemory = await vectorStoreService.Get(options.Value.CollectionName, memoryId);

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
        

        await vectorStoreService.Update(options.Value.CollectionName, memoryId,
            [..existingMemory.Vector.ToArray()], newMetadata);

        await historyService.AddHistory(memoryId, prevValue.ToString(), data, "update");

        logger.LogInformation("更新缓存 memoryId:" + memoryId + " data:" + data);
    }


    public async Task DeleteMemory(string memoryId)
    {
        var existingMemory = await vectorStoreService.Get(options.Value.CollectionName, memoryId);

        var prevValue = existingMemory.MetaData["data"];

        await vectorStoreService.Delete(options.Value.CollectionName, memoryId);

        logger.LogInformation("删除缓存 memoryId:" + memoryId);

        await historyService.AddHistory(memoryId, prevValue.ToString(), string.Empty, "delete", true);
    }
}