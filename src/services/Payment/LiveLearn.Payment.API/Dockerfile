FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5007

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/services/Payment/LiveLearn.Payment.API/LiveLearn.Payment.API.csproj", "src/services/Payment/LiveLearn.Payment.API/"]
COPY ["src/services/Payment/LiveLearn.Payment.Application/LiveLearn.Payment.Application.csproj", "src/services/Payment/LiveLearn.Payment.Application/"]
COPY ["src/services/Payment/LiveLearn.Payment.Domain/LiveLearn.Payment.Domain.csproj", "src/services/Payment/LiveLearn.Payment.Domain/"]
COPY ["src/services/Payment/LiveLearn.Payment.Infrastructure/LiveLearn.Payment.Infrastructure.csproj", "src/services/Payment/LiveLearn.Payment.Infrastructure/"]
COPY ["src/Shared/LiveLearn.BuildingBlocks/LiveLearn.BuildingBlocks.csproj", "src/Shared/LiveLearn.BuildingBlocks/"]
COPY ["src/Shared/LiveLearn.Contracts/LiveLearn.Contracts.csproj", "src/Shared/LiveLearn.Contracts/"]
RUN dotnet restore "src/services/Payment/LiveLearn.Payment.API/LiveLearn.Payment.API.csproj"
COPY . .
WORKDIR "/src/services/Payment/LiveLearn.Payment.API"
RUN dotnet build "LiveLearn.Payment.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LiveLearn.Payment.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "LiveLearn.Payment.API.dll" ]
