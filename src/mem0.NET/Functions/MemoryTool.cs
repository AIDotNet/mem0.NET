using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using mem0.NET.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0001

namespace mem0.NET.Functions;

public sealed class MemoryTool : IDisposable
{
    private readonly MemoryToolService memoryToolService;
    private readonly IServiceScope scope;

    public MemoryTool(
        IServiceProvider serviceProvider)
    {
        scope = serviceProvider.CreateScope();

        this.memoryToolService = scope.ServiceProvider.GetRequiredService<MemoryToolService>();
    }


    [KernelFunction, Description("add a memory")]
    public async Task AddMemory([Required] [Description("Data to add to memory")] string data)
    {
        await memoryToolService.AddMemoryAsync(data);
    }

    [KernelFunction, Description("Update memory provided Id and data")]
    public async Task UpdateMemory([Required] [Description("memoryid of the memory to update")] string memoryId,
        [Required] [Description("Updated data for the memory")]
        string data)
    {
        await memoryToolService.UpdateMemoryAsync(Guid.Parse(memoryId), data);
    }


    [KernelFunction, Description("Delete memory by memory_id")]
    public async Task DeleteMemory([Required] [Description("memoryid of the memory to update")] string memoryId)
    {
        await memoryToolService.DeleteMemoryAsync(Guid.Parse(memoryId));
    }

    public void Dispose()
    {
        scope.Dispose();
    }
}