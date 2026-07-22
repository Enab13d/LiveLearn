FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/services/Catalog/LiveLearn.Catalog.API/LiveLearn.Catalog.API.csproj", "src/services/Catalog/LiveLearn.Catalog.API/"]
COPY ["src/services/Catalog/LiveLearn.Catalog.Application/LiveLearn.Catalog.Application.csproj", "src/services/Catalog/LiveLearn.Catalog.Application/"]
COPY ["src/services/Catalog/LiveLearn.Catalog.Domain/LiveLearn.Catalog.Domain.csproj", "src/services/Catalog/LiveLearn.Catalog.Domain/"]
COPY ["src/services/Catalog/LiveLearn.Catalog.Infrastructure/LiveLearn.Catalog.Infrastructure.csproj", "src/services/Catalog/LiveLearn.Catalog.Infrastructure/"]
COPY ["src/Shared/LiveLearn.BuildingBlocks/LiveLearn.BuildingBlocks.csproj", "src/Shared/LiveLearn.BuildingBlocks/"]
COPY ["src/Shared/LiveLearn.Contracts/LiveLearn.Contracts.csproj", "src/Shared/LiveLearn.Contracts/"]
RUN dotnet restore "src/services/Catalog/LiveLearn.Catalog.API/LiveLearn.Catalog.API.csproj"
COPY . .
WORKDIR "/src/services/Catalog/LiveLearn.Catalog.API"
RUN dotnet build "LiveLearn.Catalog.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LiveLearn.Catalog.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "LiveLearn.Catalog.API.dll" ]