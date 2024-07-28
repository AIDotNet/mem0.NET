using mem0.Core.Model;

namespace mem0.Core.VectorStores;

public interface IHistoryService
{
    Task AddHistory(string memoryId, string prevValue, string newValue, string @event, bool isDeleted = false);
    
    Task<List<History>> GetHistories(string memoryId);
    
    Task ResetHistory();
}