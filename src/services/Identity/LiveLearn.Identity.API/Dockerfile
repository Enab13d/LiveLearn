# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5002


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/services/Identity/LiveLearn.Identity.API/LiveLearn.Identity.API.csproj", "src/services/Identity/LiveLearn.Identity.API/"]
COPY ["src/services/Identity/LiveLearn.Identity.Application/LiveLearn.Identity.Application.csproj", "src/services/Identity/LiveLearn.Identity.Application/"]
COPY ["src/services/Identity/LiveLearn.Identity.Domain/LiveLearn.Identity.Domain.csproj", "src/services/Identity/LiveLearn.Identity.Domain/"]
COPY ["src/services/Identity/LiveLearn.Identity.Infrastructure/LiveLearn.Identity.Infrastructure.csproj", "src/services/Identity/LiveLearn.Identity.Infrastructure/"]
COPY ["src/Shared/LiveLearn.BuildingBlocks/LiveLearn.BuildingBlocks.csproj", "src/Shared/LiveLearn.BuildingBlocks/"]
RUN dotnet restore "src/services/Identity/LiveLearn.Identity.API/LiveLearn.Identity.API.csproj"
COPY . .
WORKDIR "/src/services/Identity/LiveLearn.Identity.API"
RUN dotnet build "LiveLearn.Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LiveLearn.Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LiveLearn.Identity.API.dll"]