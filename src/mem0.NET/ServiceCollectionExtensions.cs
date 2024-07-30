using mem0.Core;
using mem0.NET;
using mem0.NET.Functions;
using mem0.NET.Options;
using mem0.NET.Services;
using Microsoft.SemanticKernel;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static Mem0Builder AddMem0DotNet(this IServiceCollection services,
        Action<Mem0DotNetOptions> mem0DotNetOptions)
    {
        var options = new Mem0DotNetOptions();
        mem0DotNetOptions(options);

        services.AddTransient<MemoryService>();
        
        services.AddSingleton<Mem0DotNetOptions>((_) => options);
        var openAiHttpClientHandler = new OpenAIHttpClientHandler(options.OpenAIEndpoint);

        var openAiHttpClient = new HttpClient(openAiHttpClientHandler);

#pragma warning disable SKEXP0010
        var kernelBuilder = services.AddKernel()
            .AddOpenAIChatCompletion(options.OpenAIChatCompletionModel, options.OpenAIKey,
                httpClient: openAiHttpClient)
            .AddOpenAITextEmbeddingGeneration(options.OpenAITextEmbeddingModel, options.OpenAIKey,
                httpClient: openAiHttpClient);

        kernelBuilder.Plugins.AddFromType<MemoryTool>();

#pragma warning restore SKEXP0010

        services.AddTransient<MemoryTool>();

        return new Mem0Builder(services);
    }
}