using System.Drawing;
using System.Text.Json;
using mem0.Core;
using mem0.Core.Model;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Distance = Qdrant.Client.Grpc.Distance;
using MDistance = mem0.Core.Distance;
using OptimizerStatus = mem0.Core.Model.OptimizerStatus;

namespace mem0.NET.Qdrabt.Services;

public class QdrantVectorStoresService(QdrantClient client) : IVectorStoreService
{
    public async Task CreateCol(string name, ulong vectorSize, MDistance distance = MDistance.Cosine)
    {
        if (await client.CollectionExistsAsync(name))
        {
            return;
        }

        await client.CreateCollectionAsync(name, new VectorParams()
        {
            Size = vectorSize,
            Distance = ToQdrantDistance(distance)
        });
    }

    public async Task Insert(string name, List<List<float>> vectors, List<Dictionary<string, object>> payloads = null,
        List<object>? ids = null)
    {
        var points = vectors.Select((vector, index) =>
        {
            var item = new PointStruct
            {
                Vectors = vector.ToArray(),
            };

            if (ids != null && ids.Count > index)
            {
                var id = ids[index];
                if (id is ulong u)
                {
                    item.Id = u;
                }
                else if (id is int i)
                {
                    item.Id = (ulong)i;
                }
                else if (id is long l)
                {
                    item.Id = (ulong)l;
                }
                else if (id is string s)
                {
                    item.Id = Guid.Parse(s);
                }
            }

            foreach (var payload in payloads[index])
            {
                if (payload.Value is string str)
                {
                    item.Payload.Add(payload.Key, str);
                }
                else if (payload.Value is float f)
                {
                    item.Payload.Add(payload.Key, f);
                }
                else if (payload.Value is int i)
                {
                    item.Payload.Add(payload.Key, i);
                }
                else if (payload.Value is bool b)
                {
                    item.Payload.Add(payload.Key, b);
                }
                else if (payload.Value is Color color)
                {
                    item.Payload.Add(payload.Key, color.ToArgb());
                }
                else if (payload.Value is DateTime dateTime)
                {
                    item.Payload.Add(payload.Key, dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    item.Payload.Add(payload.Key, JsonSerializer.Serialize(payload.Value));
                }
            }

            return item;
        }).ToList();

        await client.UpsertAsync(name, points);
    }

    private Distance ToQdrantDistance(MDistance distance)
    {
        return distance switch
        {
            MDistance.Cosine => Distance.Cosine,
            MDistance.UnknownDistance => Distance.UnknownDistance,
            MDistance.Euclid => Distance.Euclid,
            MDistance.Dot => Distance.Dot,
            MDistance.Manhattan => Distance.Manhattan,
            _ => throw new ArgumentOutOfRangeException(nameof(distance), distance, null)
        };
    }

    public async Task<List<SearchHit>> Search(string name, float[] query, ulong limit = 5UL,
        Dictionary<string, object> filters = null)
    {
        var filter = CreateFilter(filters);
        var hits = await client.SearchAsync(name, query.ToArray(), filter, limit: limit);

        return hits.Select(hit => new SearchHit
        {
            Id = hit.Id,
            Score = hit.Score,
            Payload = hit.Payload.ToDictionary(x => x.Key, x => (object)x.Value),
        }).ToList();
    }

    private Filter CreateFilter(Dictionary<string, object> filters)
    {
        var conditions = new List<Condition>();

        foreach (var filter in filters)
        {
            if (filter.Value is Dictionary<string, object> rangeDict &&
                rangeDict.ContainsKey("gte") && rangeDict.TryGetValue("lte", out var value))
            {
                conditions.Add(new Condition
                {
                    Field = new FieldCondition()
                    {
                        Key = filter.Key,
                        Match = new Match
                        {
                            Text = value.ToString()
                        }
                    },
                });
            }
            else
            {
                conditions.Add(new Condition()
                {
                    Field = new FieldCondition
                    {
                        Key = filter.Key,
                        Match = new Match
                        {
                            Text = filter.Value.ToString()
                        }
                    }
                });
            }
        }

        var filterValue = new Filter()
        {
            Must =
            {
                conditions
            }
        };
        return conditions.Any() ? filterValue : null;
    }

    public async Task Delete(string name, object vectorId)
    {
        if (vectorId is ulong u)
        {
            await client.DeleteAsync(name, u);
        }
        else if (vectorId is int i)
        {
            await client.DeleteAsync(name, (ulong)i);
        }
        else if (vectorId is long l)
        {
            await client.DeleteAsync(name, (ulong)l);
        }
        else if (vectorId is string s)
        {
            await client.DeleteAsync(name, Guid.Parse(s));
        }
    }

    public async Task Update(string name, object vectorId, List<float> vector = null,
        Dictionary<string, object> payload = null)
    {
        var pointId = vectorId switch
        {
            ulong u => new PointId { Num = u },
            int i => new PointId { Num = (ulong)i },
            long l => new PointId { Num = (ulong)l },
            string s => new PointId { Uuid = s },
            _ => throw new ArgumentOutOfRangeException(nameof(vectorId), vectorId, null)
        };
        var point = new PointStruct()
        {
            Id = pointId,
            Vectors = vector?.ToArray()
        };

        if (payload != null)
        {
            foreach (var item in payload)
            {
                if (item.Value is string str)
                {
                    point.Payload.Add(item.Key, str);
                }
                else if (item.Value is float f)
                {
                    point.Payload.Add(item.Key, f);
                }
                else if (item.Value is int i)
                {
                    point.Payload.Add(item.Key, i);
                }
                else if (item.Value is bool b)
                {
                    point.Payload.Add(item.Key, b);
                }
                else if (item.Value is Color color)
                {
                    point.Payload.Add(item.Key, color.ToArgb());
                }
                else
                {
                    point.Payload.Add(item.Key, JsonSerializer.Serialize(item.Value));
                }
            }
        }

        await client.UpsertAsync(name, new List<PointStruct> { point });
    }

    public async Task<VectorData> Get(string name, object vectorId)
    {
        var pointId = vectorId switch
        {
            ulong u => new PointId { Num = u },
            int i => new PointId { Num = (ulong)i },
            long l => new PointId { Num = (ulong)l },
            string s => new PointId { Uuid = s },
            _ => throw new ArgumentOutOfRangeException(nameof(vectorId), vectorId, null)
        };

        var result = (await client.RetrieveAsync(name, ids: new List<PointId>()
        {
            pointId
        },true,true)).FirstOrDefault();

        return new VectorData()
        {
            Id = result.Id.Num,
            Vector = result.Vectors.Vector.Data.ToList(),
            MetaData = result.Payload.ToDictionary(x => x.Key, x => (object)x.Value)
        };
    }

    public async Task<IReadOnlyList<string>> ListCols()
    {
        return await client.ListCollectionsAsync();
    }

    public async Task DeleteCol(string name)
    {
        await client.DeleteCollectionAsync(name);
    }

    public async Task<VectorInfo> ColInfo(string name)
    {
        var result = await client.GetCollectionInfoAsync(name);

        return new VectorInfo()
        {
            HasVectorsCount = result.HasVectorsCount,
            Status = (VectorInfoStatus)((int)result.Status),
            OptimizerStatus = new OptimizerStatus()
            {
                Ok = result.OptimizerStatus.Ok,
                Error = result.OptimizerStatus.Error
            },
            HasPointsCount = result.HasPointsCount,
            PayloadSchema = result.PayloadSchema.ToDictionary(x => x.Key, x => (object)x.Value),
            PointsCount = result.PointsCount,
            SegmentsCount = result.SegmentsCount
        };
    }

    public async Task<List<VectorData>> List(string name, Dictionary<string, object> filters = null, uint limit = 100U)
    {
        var filter = CreateFilter(filters);

        var result = await client.ScrollAsync(name, filter, limit: limit);

        return result.Result.Select(hit => new VectorData()
        {
            Id = hit.Id.Num,
            Vector = hit.Vectors.Vector.Data,
            MetaData = hit.Payload.ToDictionary(x => x.Key, x => (object)x.Value)
        }).ToList();
    }
}