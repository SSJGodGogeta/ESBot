FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ESBot.API/ESBot.API.csproj", "ESBot.API/"]
COPY ["ESBot.Domain/ESBot.Domain.csproj", "ESBot.Domain/"]
COPY ["ESBot.Infrastructure/ESBot.Infrastructure.csproj", "ESBot.Infrastructure/"]
RUN dotnet restore "ESBot.API/ESBot.API.csproj"
COPY . .
WORKDIR "/src/ESBot.API"
RUN dotnet build "ESBot.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ESBot.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

USER $APP_UID

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 CMD curl -f http://localhost:8080/health/ready || exit 1
ENTRYPOINT ["dotnet", "ESBot.API.dll"]
