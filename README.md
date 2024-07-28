# 简介

mem0.NET是用[mem0](https://github.com/mem0ai/mem0)的python版本移植成.NET版本
对于mem0.NET的框架设计我们看的非常重要，并且对于核心的功能我们进行拆分，以便用户组成自己各种的实现方式，

```sheel
--src
  --mem0.Core 是项目的核心，也是一些框架共享和接口抽象的类库
  --mem0.NET 是项目主要实现，提供了核心Function
  --mem0.NET.EntityFramework 是项目存储的EFCore的实现
  --mem0.NET.Qadrant 是项目中向量数据库的Qadrant的实现
  --mem0.NET.Service 是项目默认提供的WebAPI服务实现，默认使用`postgres`数据库存储记忆
```

