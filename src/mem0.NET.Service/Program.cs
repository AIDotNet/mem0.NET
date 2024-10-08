using mem0.NET.Options;
using mem0.NET.Service.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Options;

namespace mem0.NET.Service;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddOptions<Mem0Options>()
            .Bind(builder.Configuration.GetSection("Mem0"));

        builder.Services.AddOptions<QdrantOptions>()
            .Bind(builder.Configuration.GetSection("Qdrant"));

        var options = builder.Configuration.GetSection("Mem0")
            .Get<Mem0Options>();

        var qdrantOptions = builder.Configuration.GetSection("Qdrant")
            .Get<QdrantOptions>();

        builder.Services.AddMem0DotNet(options)
            .WithMem0EntityFrameworkCore<MasterDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
            })
            .WithVectorQdrant(qdrantOptions);

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<MasterDbContext>();

            await dbContext.Database.EnsureCreatedAsync();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapMemoryService();

        await app.RunAsync();
    }
}