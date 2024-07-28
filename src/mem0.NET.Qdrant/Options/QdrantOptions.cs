namespace Microsoft.Extensions.DependencyInjection.Options;

public class QdrantOptions
{
    public string Host { get; set; }

    public int Port { get; set; } = 6334;

    public bool Https { get; set; }

    public string ApiKey { get; set; }

    public TimeSpan GrpcTimeout { get; set; } = default;
}