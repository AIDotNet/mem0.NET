namespace mem0.NET;

/// <summary>
/// OpenAI HttpClientHandler
/// </summary>
/// <param name="uri"></param>
public sealed class OpenAIHttpClientHandler(string uri) : HttpClientHandler
{
    private readonly string _uri = uri.TrimEnd('/');

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.RequestUri = new Uri(request.RequestUri.ToString().Replace("https://api.openai.com", _uri));

        var response =  await base.SendAsync(request, cancellationToken);
        
        return response;
    }
}