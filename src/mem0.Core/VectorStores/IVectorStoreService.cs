using mem0.Core.Model;

namespace mem0.Core;

public interface IVectorStoreService
{
    public Task CreateCol(string name, ulong vectorSize, Distance distance = Distance.Cosine);

    public Task Insert(string name, List<List<float>> vectors, List<Dictionary<string, object>> payloads = null,
        List<object> ids = null);

    public Task<List<SearchHit>> Search(string name, float[] query, ulong limit = 5UL,
        Dictionary<string, object> filters = null);

    public Task Delete(string name, object vectorId);

    public Task Update(string name, object vectorId, List<float> vector = null,
        Dictionary<string, object> payload = null);

    public Task<VectorData> Get(string name, object vectorId);
    public Task<IReadOnlyList<string>> ListCols();
    public Task DeleteCol(string name);
    public Task<VectorInfo> ColInfo(string name);

    public Task<List<VectorData>> List(string name, Dictionary<string, object> filters = null, uint limit = 100U);
}