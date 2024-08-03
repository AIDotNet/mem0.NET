using mem0.Core;
using mem0.Core.Model;
using mem0.Core.VectorStores;
using mem0.NET.Service.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace mem0.NET.EntityFramework.Services;

/// <summary>
/// 
/// </summary>
/// <param name="dbContext"></param>
/// <typeparam name="TDbContext"></typeparam>
public sealed class HistoryService<TDbContext>(TDbContext dbContext)
    : IHistoryService where TDbContext : Mem0DbContext<TDbContext>
{
    public async Task AddHistoryAsync(string memoryId, string prevValue, string newValue, string @event,
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

    /// <inheritdoc />
    public async Task<PagingDto<History>> GetHistoriesAsync(string memoryId, int page = 1, int pageSize = 10)
    {
        var total = await dbContext.Histories.Where(h => h.MemoryId == memoryId).CountAsync();

        var histories = await dbContext.Histories.Where(h => h.MemoryId == memoryId)
            .OrderByDescending(h => h.DateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagingDto<History>
        {
            Total = total,
            Items = histories
        };
    }

    /// <inheritdoc />
    public async Task ResetHistoryAsync()
    {
        await dbContext.Histories
            .ExecuteDeleteAsync();
    }
}