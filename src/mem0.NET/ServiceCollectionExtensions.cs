using mem0.Core;
using mem0.Core.VectorStores;
using mem0.NET;
using mem0.NET.EntityFramework.Services;
using mem0.NET.Functions;
using mem0.NET.Options;
using mem0.NET.Service.DataAccess;
using mem0.NET.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0010

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static Mem0Builder AddMem0DotNet(this IServiceCollection services, Mem0Options value,
        Action<DbContextOptionsBuilder> optionsAction, HttpClientHandler? openAiHttpClientHandler = null)
    {
        services.AddSingleton<MemoryService>();
        services.AddSingleton<MemoryToolService>();

        var handler = openAiHttpClientHandler ?? new OpenAIHttpClientHandler(value.OpenAIEndpoint);

        var openAiHttpClient = new HttpClient(handler);

        var kernelBuilder = services.AddKernel()
            .AddOpenAIChatCompletion(value.OpenAIChatCompletionModel, value.OpenAIKey,
                httpClient: openAiHttpClient)
            .AddOpenAITextEmbeddingGeneration(value.OpenAITextEmbeddingModel, value.OpenAIKey,
                httpClient: openAiHttpClient);

        kernelBuilder.Services.AddSingleton<MemoryTool>();

        kernelBuilder.Plugins.AddFromType<MemoryTool>();

        services.AddDbContext<Mem0DbContext>(optionsAction);

        services.AddSingleton<IHistoryService, HistoryService>();

        return new Mem0Builder(services);
    }
}