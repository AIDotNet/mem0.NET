using mem0.Core.Model;

namespace mem0.Core.VectorStores;

/// <summary>
/// 历史记录服务
/// </summary>
public interface IHistoryService
{
    /// <summary>
    /// 添加历史记录
    /// </summary>
    /// <param name="memoryId">缓存id</param>
    /// <param name="prevValue">旧数据</param>
    /// <param name="newValue">新数据</param>
    /// <param name="event">事件类型</param>
    /// <param name="isDeleted">是否删除</param>
    /// <returns></returns>
    Task AddHistoryAsync(string memoryId, string prevValue, string newValue, string @event, bool isDeleted = false);

    /// <summary>
    /// 获取历史记录
    /// </summary>
    /// <param name="memoryId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<PagingDto<History>> GetHistoriesAsync(string memoryId, int page = 1, int pageSize = 10);
    
    /// <summary>
    /// 刷写历史记录
    /// </summary>
    /// <returns></returns>
    Task ResetHistoryAsync();
}