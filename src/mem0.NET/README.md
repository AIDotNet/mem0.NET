# 简单入门教程

安装mem0.NET

```bash
dotnet add package mem0.NET
dotnet add package mem0.EntityFrameworkCore
dotnet add mem0.NET.Qdrant
```

集成到项目

打开`appsettings.json`文件，添加以下配置

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=test;Username=token;Password=dd666666"
  },
  "Mem0": {
    "OpenAIEndpoint": "https://api.token-ai.cn",
    "OpenAIKey": "sk-666666",
    "OpenAIChatCompletionModel": "gpt-4o-mini",
    "OpenAITextEmbeddingModel": "text-embedding-ada-002",
    "CollectionName": "mem0-test"
  },
  "Qdrant": {
    "Host": "127.0.0.1",
    "Port": 6334,
    "Https": false,
    "ApiKey": "dd666666"
  }
}
```

创建一个EFCore的DbContext,需要继承`Mem0DbContext`类，如下所示
`MasterDbContext.cs`
```csharp
using mem0.NET.Service.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace mem0.NET.Service;

public class MasterDbContext(DbContextOptions<MasterDbContext> options) : Mem0DbContext<MasterDbContext>(options)
{
}
```
然后打开`Program.cs`文件，添加以下代码
```csharp
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
```

然后使用，如下所示，创建缓存到向量数据库
```csharp
public TestService(MemoryService memoryService)
{
    public async Task CreateAsync(CreateMemoryInput input)
    {
    }
}
```
搜索向量服务，
```csharp

public TestService(MemoryService memoryService)
{
    public async Task SearchAsync(string query, string? userId,
        string? agentId, string? runId, uint limit = 10)
    {
        memoryService.SearchMemory(query, userId, agentId, runId, limit);
    }
}
```

