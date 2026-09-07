FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5004

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/services/Assessment/LiveLearn.Assessment.API/LiveLearn.Assessment.API.csproj", "src/services/Assessment/LiveLearn.Assessment.API/"]
COPY ["src/services/Assessment/LiveLearn.Assessment.Application/LiveLearn.Assessment.Application.csproj", "src/services/Assessment/LiveLearn.Assessment.Application/"]
COPY ["src/services/Assessment/LiveLearn.Assessment.Domain/LiveLearn.Assessment.Domain.csproj", "src/services/Assessment/LiveLearn.Assessment.Domain/"]
COPY ["src/services/Assessment/LiveLearn.Assessment.Infrastructure/LiveLearn.Assessment.Infrastructure.csproj", "src/services/Assessment/LiveLearn.Assessment.Infrastructure/"]
COPY ["src/Shared/LiveLearn.BuildingBlocks/LiveLearn.BuildingBlocks.csproj", "src/Shared/LiveLearn.BuildingBlocks/"]
COPY ["src/Shared/LiveLearn.Contracts/LiveLearn.Contracts.csproj", "src/Shared/LiveLearn.Contracts/"]
RUN dotnet restore "src/services/Assessment/LiveLearn.Assessment.API/LiveLearn.Assessment.API.csproj"
COPY . .
WORKDIR "/src/services/Assessment/LiveLearn.Assessment.API"
RUN dotnet build "LiveLearn.Assessment.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LiveLearn.Assessment.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "LiveLearn.Assessment.API.dll" ]