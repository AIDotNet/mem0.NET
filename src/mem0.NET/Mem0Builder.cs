using Microsoft.Extensions.DependencyInjection;

namespace mem0.Core;

public class Mem0Builder
{
    public IServiceCollection Services { get; }

    public Mem0Builder(IServiceCollection services)
    {
        Services = services;
    }
}