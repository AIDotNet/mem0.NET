using mem0.Core.Model;

namespace mem0.Core;

public interface IVectorStoreService
{
    public Task CreateColAsync(string name, ulong vectorSize, Distance distance = Distance.Cosine);

    /// <summary>
    /// Insert vectors
    /// </summary>
    /// <param name="name"></param>
    /// <param name="vectors"></param>
    /// <param name="payloads"></param>
    /// <param name="ids"></param>
    /// <returns></returns>
    public Task InsertAsync(string name, List<List<float>> vectors, List<Dictionary<string, object>> payloads = null,
        List<Guid> ids = null);

    /// <summary>
    /// Search for vectors
    /// </summary>
    /// <param name="name"></param>
    /// <param name="query"></param>
    /// <param name="limit"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public Task<List<SearchHit>> SearchAsync(string name, float[] query, ulong limit = 5UL,
        Dictionary<string, object>? filters = null);

    /// <summary>
    /// Delete vector
    /// </summary>
    /// <param name="name"></param>
    /// <param name="vectorId"></param>
    /// <returns></returns>
    public Task DeleteAsync(string name, Guid vectorId);

    /// <summary>
    /// Update vector
    /// </summary>
    /// <param name="name"></param>
    /// <param name="vectorId"></param>
    /// <param name="vector"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    public Task UpdateAsync(string name, Guid vectorId, List<float> vector = null,
        Dictionary<string, string> payload = null);

    /// <summary>
    /// Get vector
    /// </summary>
    /// <param name="name"></param>
    /// <param name="vectorId"></param>
    /// <returns></returns>
    public Task<VectorData> GetAsync(string name, Guid vectorId);
    
    /// <summary>
    /// List collections
    /// </summary>
    /// <returns></returns>
    public Task<IReadOnlyList<string>> ListColsAsync();
    
    /// <summary>
    /// Delete collection
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task DeleteColAsync(string name);
    
    /// <summary>
    /// Get collection info
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<VectorInfo> ColInfoAsync(string name);

    /// <summary>
    /// List vectors in a collection
    /// </summary>
    /// <param name="name"></param>
    /// <param name="filters"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public Task<List<VectorData>> GetListAsync(string name, Dictionary<string, object>? filters = null, uint limit = 100U);
}