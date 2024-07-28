FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/mem0.NET.Service/mem0.NET.Service.csproj", "src/mem0.NET.Service/"]
COPY ["src/mem0.Core/mem0.Core.csproj", "src/mem0.Core/"]
COPY ["src/mem0.NET.Qdrant/mem0.NET.Qdrant.csproj", "src/mem0.NET.Qdrant/"]
COPY ["src/mem0.NET/mem0.NET.csproj", "src/mem0.NET/"]
RUN dotnet restore "src/mem0.NET.Service/mem0.NET.Service.csproj"
COPY . .
WORKDIR "/src/src/mem0.NET.Service"
RUN dotnet build "mem0.NET.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "mem0.NET.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mem0.NET.Service.dll"]
