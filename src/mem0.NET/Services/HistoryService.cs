using mem0.Core.Model;
using mem0.Core.VectorStores;
using mem0.NET.Service.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace mem0.NET.EntityFramework.Services;

public sealed class HistoryService(IServiceProvider serviceProvider) : IHistoryService
{
    AsyncLocal<(IServiceScope, Mem0DbContext)?> dbContext = new((args =>
    {
        if (args.PreviousValue != null)
        {
            args.PreviousValue.Value.Item2.Dispose();
            args.PreviousValue.Value.Item1.Dispose();
        }
    }));

    public Mem0DbContext GetDbContext()
    {
        if (dbContext.Value == null)
        {
            var scope = serviceProvider.CreateScope();
            dbContext.Value = (scope, scope.ServiceProvider.GetRequiredService<Mem0DbContext>());
        }

        return dbContext.Value.Value.Item2;
    }

    public async Task AddHistory(string memoryId, string prevValue, string newValue, string @event,
        bool isDeleted = false)
    {
        var dbContext = GetDbContext();

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
        var dbContext = GetDbContext();

        var histories = await dbContext.Histories.Where(h => h.MemoryId == memoryId).ToListAsync();

        return histories;
    }

    public async Task ResetHistory()
    {
        var dbContext = GetDbContext();
        await dbContext.Histories
            .ExecuteDeleteAsync();
    }
}