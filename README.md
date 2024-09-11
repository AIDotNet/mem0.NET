# 简介

mem0.NET是用[mem0](https://github.com/mem0ai/mem0)的python版本移植成.NET版本
对于mem0.NET的框架设计我们看的非常重要，并且对于核心的功能我们进行拆分，以便用户组成自己各种的实现方式，

```sheel
--src
  --mem0.NET 是项目主要实现，提供了核心Function
  --mem0.NET.Qadrant 是项目中向量数据库的Qadrant的实现
  --mem0.EntityFrameworkCore 是EFCore的数据库实现
  --mem0.FreeSql 是FreeSql的数据实现
  --mem0.NET.Service 是项目默认提供的WebAPI服务实现，默认使用`postgres`数据库存储记忆
```

## 🔑 核心功能

- 多级内存：用户、会话和 AI 代理内存保留
- 自适应个性化：基于交互的持续改进
- 开发人员友好的 API：轻松集成到各种应用程序中
- 跨平台一致性：跨设备的统一行为
- 托管服务：无忧托管解决方案

## 🚀 快速开始

### 简单入门教程

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

### 安装Qdrant

```bash
docker run -d --name qdrant -p 6334:6334 -v /path/to/data:/data registry.cn-hangzhou.aliyuncs.com/aidotnet/qdrant
```
