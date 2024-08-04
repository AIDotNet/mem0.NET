using mem0.Core;
using mem0.NET.Qdrabt.Services;
using Microsoft.Extensions.DependencyInjection.Options;
using Qdrant.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册Vector Qdrant实现
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static Mem0Builder WithVectorQdrant(this Mem0Builder services,
        QdrantOptions options)
    {
        services.Services.AddScoped(_ =>
        {
            var client = new QdrantClient(options.Host, options.Port, options.Https, options.ApiKey,
                options.GrpcTimeout);

            return client;
        });

        services.Services.AddScoped<IVectorStoreService, QdrantVectorStoresService>();

        return services;
    }
}