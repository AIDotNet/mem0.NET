using mem0.Core.Model;
using mem0.Core.VectorStores;
using mem0.NET.Service.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace mem0.NET.EntityFramework.Services;

public sealed class HistoryService(MasterDbContext dbContext) : IHistoryService
{
    public async Task AddHistory(string memoryId, string prevValue, string newValue, string @event,
        bool isDeleted = false)
    {
        await dbContext.Histories.AddAsync(new History
        {
            MemoryId = memoryId,
            PrevValue = prevValue,
            NewValue = newValue,
            Event = @event,
            DateTime = DateTime.Now,
            IsDeleted = isDeleted
        });
    }

    public async Task<List<History>> GetHistories(string memoryId)
    {
        var histories = await dbContext.Histories.Where(h => h.MemoryId == memoryId).ToListAsync();

        return histories;
    }

    public async Task ResetHistory()
    {
        await dbContext.Histories
            .ExecuteDeleteAsync();
    }
}