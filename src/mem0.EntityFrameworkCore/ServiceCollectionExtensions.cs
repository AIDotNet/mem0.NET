using mem0.Core;
using mem0.Core.VectorStores;
using mem0.NET.EntityFramework.Services;
using mem0.NET.Service.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Mem0.NET 数据库服务
    /// </summary>
    /// <param name="kernelBuilder"></param>
    /// <param name="optionsAction"></param>
    /// <returns></returns>
    public static Mem0Builder WithMem0EntityFrameworkCore<TDbContext>(this Mem0Builder kernelBuilder,
        Action<DbContextOptionsBuilder> optionsAction) where TDbContext : Mem0DbContext<TDbContext>
    {
        kernelBuilder.Services.AddDbContext<TDbContext>(optionsAction);

        kernelBuilder.Services.AddScoped<IHistoryService, HistoryService<TDbContext>>();

        return kernelBuilder;
    }
}