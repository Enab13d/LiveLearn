FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5006

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/services/Ordering/LiveLearn.Ordering.API/LiveLearn.Ordering.API.csproj", "src/services/Ordering/LiveLearn.Ordering.API/"]
COPY ["src/services/Ordering/LiveLearn.Ordering.Application/LiveLearn.Ordering.Application.csproj", "src/services/Ordering/LiveLearn.Ordering.Application/"]
COPY ["src/services/Ordering/LiveLearn.Ordering.Domain/LiveLearn.Ordering.Domain.csproj", "src/services/Ordering/LiveLearn.Ordering.Domain/"]
COPY ["src/services/Ordering/LiveLearn.Ordering.Infrastructure/LiveLearn.Ordering.Infrastructure.csproj", "src/services/Ordering/LiveLearn.Ordering.Infrastructure/"]
COPY ["src/Shared/LiveLearn.BuildingBlocks/LiveLearn.BuildingBlocks.csproj", "src/Shared/LiveLearn.BuildingBlocks/"]
COPY ["src/Shared/LiveLearn.Contracts/LiveLearn.Contracts.csproj", "src/Shared/LiveLearn.Contracts/"]
RUN dotnet restore "src/services/Ordering/LiveLearn.Ordering.API/LiveLearn.Ordering.API.csproj"
COPY . .
WORKDIR "/src/services/Ordering/LiveLearn.Ordering.API"
RUN dotnet build "LiveLearn.Ordering.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LiveLearn.Ordering.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "LiveLearn.Ordering.API.dll" ]