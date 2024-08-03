using FreeSql;
using mem0.Core;
using mem0.Core.VectorStores;
using mem0.NET.EntityFramework.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Mem0.NET FreeSql 服务
    /// </summary>
    /// <param name="kernelBuilder"></param>
    /// <param name="builderAction"></param>
    /// <returns></returns>
    public static Mem0Builder WithMem0FreeSql(this Mem0Builder kernelBuilder,
        Action<FreeSqlBuilder> builderAction)
    {
        kernelBuilder.Services.AddScoped<IFreeSql>(_ =>
        {
            var freeSql = new FreeSqlBuilder();
            builderAction(freeSql);

            return freeSql.Build();
        });

        kernelBuilder.Services.AddScoped<IHistoryService, HistoryService>();

        return kernelBuilder;
    }
}