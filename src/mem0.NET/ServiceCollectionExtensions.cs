using mem0.Core;
using mem0.NET;
using mem0.NET.Functions;
using mem0.NET.Options;
using mem0.NET.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0010

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Mem0.NET 服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="value"></param>
    /// <param name="openAiHttpClientHandler"></param>
    /// <returns></returns>
    public static Mem0Builder AddMem0DotNet(this IServiceCollection services, Mem0Options value,
        HttpClientHandler? openAiHttpClientHandler = null)
    {
        services.AddScoped<MemoryService>();
        services.AddScoped<MemoryToolService>();

        var handler = openAiHttpClientHandler ?? new OpenAIHttpClientHandler(value.OpenAIEndpoint);

        var openAiHttpClient = new HttpClient(handler);

        var kernelBuilder = services
            .AddKernel()
            .AddOpenAIChatCompletion(value.OpenAIChatCompletionModel, value.OpenAIKey,
                httpClient: openAiHttpClient)
            .AddOpenAITextEmbeddingGeneration(value.OpenAITextEmbeddingModel, value.OpenAIKey,
                httpClient: openAiHttpClient);
        
        kernelBuilder.Services.AddScoped<MemoryTool>();
        
        kernelBuilder.Plugins.AddFromType<MemoryTool>();


        return new Mem0Builder(services);
    }
}