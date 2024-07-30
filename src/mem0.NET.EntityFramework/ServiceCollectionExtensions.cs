using mem0.Core;
using mem0.Core.VectorStores;
using mem0.NET.EntityFramework.Services;
using mem0.NET.Service.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMem0DotNetEntityFramework(this IServiceCollection services,
        Action<DbContextOptionsBuilder>? optionsAction = null)
    {
        services.AddDbContext<Mem0DbContext>(optionsAction);

        services.AddScoped<IHistoryService, HistoryService>();
        
        return services;
    }
}