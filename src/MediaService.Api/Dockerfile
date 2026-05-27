FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MediaService.Api/MediaService.Api.csproj", "src/MediaService.Api/"]
COPY ["src/MediaService.Infrastructure/MediaService.Infrastructure.csproj", "src/MediaService.Infrastructure/"]
COPY ["src/MediaService.Application/MediaService.Application.csproj", "src/MediaService.Application/"]
COPY ["src/Common/Common.ResultPattern/Common.ResultPattern.csproj", "src/Common/Common.ResultPattern/"]
COPY ["src/MediaService.Domain/MediaService.Domain.csproj", "src/MediaService.Domain/"]
RUN dotnet restore "src/MediaService.Api/MediaService.Api.csproj"
COPY . .
WORKDIR "/src/src/MediaService.Api"
RUN dotnet build "./MediaService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MediaService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaService.Api.dll"]
