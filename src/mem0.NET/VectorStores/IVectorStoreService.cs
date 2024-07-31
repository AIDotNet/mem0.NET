using mem0.Core.Model;

namespace mem0.Core;

public interface IVectorStoreService
{
    public Task CreateCol(string name, ulong vectorSize, Distance distance = Distance.Cosine);

    /// <summary>
    /// Insert vectors
    /// </summary>
    /// <param name="name"></param>
    /// <param name="vectors"></param>
    /// <param name="payloads"></param>
    /// <param name="ids"></param>
    /// <returns></returns>
    public Task Insert(string name, List<List<float>> vectors, List<Dictionary<string, object>> payloads = null,
        List<object> ids = null);

    /// <summary>
    /// Search for vectors
    /// </summary>
    /// <param name="name"></param>
    /// <param name="query"></param>
    /// <param name="limit"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public Task<List<SearchHit>> Search(string name, float[] query, ulong limit = 5UL,
        Dictionary<string, object> filters = null);

    /// <summary>
    /// Delete vector
    /// </summary>
    /// <param name="name"></param>
    /// <param name="vectorId"></param>
    /// <returns></returns>
    public Task Delete(string name, object vectorId);

    /// <summary>
    /// Update vector
    /// </summary>
    /// <param name="name"></param>
    /// <param name="vectorId"></param>
    /// <param name="vector"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    public Task Update(string name, object vectorId, List<float> vector = null,
        Dictionary<string, object> payload = null);

    /// <summary>
    /// Get vector
    /// </summary>
    /// <param name="name"></param>
    /// <param name="vectorId"></param>
    /// <returns></returns>
    public Task<VectorData> Get(string name, object vectorId);
    
    /// <summary>
    /// List collections
    /// </summary>
    /// <returns></returns>
    public Task<IReadOnlyList<string>> ListCols();
    
    /// <summary>
    /// Delete collection
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task DeleteCol(string name);
    
    /// <summary>
    /// Get collection info
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<VectorInfo> ColInfo(string name);

    /// <summary>
    /// List vectors in a collection
    /// </summary>
    /// <param name="name"></param>
    /// <param name="filters"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public Task<List<VectorData>> List(string name, Dictionary<string, object> filters = null, uint limit = 100U);
}