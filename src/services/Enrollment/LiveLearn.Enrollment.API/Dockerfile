FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5005

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/services/Enrollment/LiveLearn.Enrollment.API/LiveLearn.Enrollment.API.csproj", "src/services/Enrollment/LiveLearn.Enrollment.API/"]
COPY ["src/services/Enrollment/LiveLearn.Enrollment.Application/LiveLearn.Enrollment.Application.csproj", "src/services/Enrollment/LiveLearn.Enrollment.Application/"]
COPY ["src/services/Enrollment/LiveLearn.Enrollment.Domain/LiveLearn.Enrollment.Domain.csproj", "src/services/Enrollment/LiveLearn.Enrollment.Domain/"]
COPY ["src/services/Enrollment/LiveLearn.Enrollment.Infrastructure/LiveLearn.Enrollment.Infrastructure.csproj", "src/services/Enrollment/LiveLearn.Enrollment.Infrastructure/"]
COPY ["src/Shared/LiveLearn.BuildingBlocks/LiveLearn.BuildingBlocks.csproj", "src/Shared/LiveLearn.BuildingBlocks/"]
COPY ["src/Shared/LiveLearn.Contracts/LiveLearn.Contracts.csproj", "src/Shared/LiveLearn.Contracts/"]
RUN dotnet restore "src/services/Enrollment/LiveLearn.Enrollment.API/LiveLearn.Enrollment.API.csproj"
COPY . .
WORKDIR "/src/services/Enrollment/LiveLearn.Enrollment.API"
RUN dotnet build "LiveLearn.Enrollment.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LiveLearn.Enrollment.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "LiveLearn.Enrollment.API.dll" ]