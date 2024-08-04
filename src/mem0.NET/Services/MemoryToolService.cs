using mem0.Core;
using mem0.Core.VectorStores;
using mem0.NET.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Embeddings;

#pragma warning disable SKEXP0001

namespace mem0.NET.Services;

public class MemoryToolService(
    IHistoryService historyService,
    IServiceProvider serviceProvider,
    IVectorStoreService vectorStoreService,
    IOptions<Mem0Options> options)
{
    public async Task AddMemoryAsync(string data)
    {
        var currentValue = ApplicationContext.Current.Value;

        await using var scope = serviceProvider.CreateAsyncScope();
        var textEmbeddingGenerationService =
            scope.ServiceProvider.GetRequiredService<ITextEmbeddingGenerationService>();

        var embeddings = await textEmbeddingGenerationService.GenerateEmbeddingAsync(data);
        var memoryId = Guid.NewGuid();
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

        await vectorStoreService.InsertAsync(options.Value.CollectionName,
            [[..embeddings.ToArray()]],
            [metadata], [memoryId]);

        await historyService.AddHistoryAsync(memoryId.ToString(), string.Empty, data, "add");

        await Task.CompletedTask;
    }

    public async Task UpdateMemoryAsync(Guid memoryId,
        string data)
    {
        var existingMemory = await vectorStoreService.GetAsync(options.Value.CollectionName, memoryId);

        var prevValue = existingMemory.MetaData["data"];
        var newMetadata = new Dictionary<string, object>
        {
            { "data", prevValue },
            { "updated_at", DateTime.Now },
        };

        foreach (var o in existingMemory.MetaData)
        {
            if (o.Key != "data")
            {
                newMetadata[o.Key] = o.Value;
            }
        }


        await vectorStoreService.UpdateAsync(options.Value.CollectionName, memoryId,
            [..existingMemory.Vector.ToArray()], newMetadata);

        await historyService.AddHistoryAsync(memoryId.ToString(), prevValue.ToString(), data, "update");
    }


    public async Task DeleteMemoryAsync(Guid memoryId)
    {
        var existingMemory = await vectorStoreService.GetAsync(options.Value.CollectionName, memoryId);

        var prevValue = existingMemory.Text;

        await vectorStoreService.DeleteAsync(options.Value.CollectionName, memoryId);

        await historyService.AddHistoryAsync(memoryId.ToString(), prevValue, string.Empty, "delete", true);
    }
}