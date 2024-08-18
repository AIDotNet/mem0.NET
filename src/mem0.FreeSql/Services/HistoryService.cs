using mem0.Core;
using mem0.Core.Model;
using mem0.Core.VectorStores;

namespace mem0.NET.EntityFramework.Services;

/// <summary>
/// 
/// </summary>
/// <param name="dbContext"></param>
public sealed class HistoryService(IFreeSql dbContext)
    : IHistoryService
{
    public async Task AddHistoryAsync(string memoryId, string prevValue, string newValue, string @event,
        bool isDeleted = false, string? userId = null, string? trackId = null)
    {
        await dbContext.Insert<History>(new History
        {
            MemoryId = memoryId,
            PrevValue = prevValue,
            NewValue = newValue,
            Event = @event,
            DateTime = DateTime.Now,
            IsDeleted = isDeleted,
            UserId = userId,
            TrackId = trackId
        }).ExecuteAffrowsAsync();
    }

    public async Task<PagingDto<History>> GetHistoriesAsync(string memoryId, int page = 1, int pageSize = 10)
    {
        var total = await dbContext.Select<History>().Where(h => h.MemoryId == memoryId).CountAsync();

        var histories = await dbContext.Select<History>().Where(h => h.MemoryId == memoryId)
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
        await dbContext.Delete<History>()
            .Where(h => true)
            .ExecuteAffrowsAsync();
    }
}