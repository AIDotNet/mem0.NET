# 简介

mem0.NET是用[mem0](https://github.com/mem0ai/mem0)的python版本移植成.NET版本
对于mem0.NET的框架设计我们看的非常重要，并且对于核心的功能我们进行拆分，以便用户组成自己各种的实现方式，

```sheel
--src
  --mem0.NET 是项目主要实现，提供了核心Function
  --mem0.NET.Qadrant 是项目中向量数据库的Qadrant的实现
  --mem0.EntityFrameworkCore 是EFCore的数据库实现
  --mem0.FreeSql 是FreeSqk的数据实现
  --mem0.NET.Service 是项目默认提供的WebAPI服务实现，默认使用`postgres`数据库存储记忆
```

## 🔑 核心功能

- 多级内存：用户、会话和 AI 代理内存保留
- 自适应个性化：基于交互的持续改进
- 开发人员友好的 API：轻松集成到各种应用程序中
- 跨平台一致性：跨设备的统一行为
- 托管服务：无忧托管解决方案
