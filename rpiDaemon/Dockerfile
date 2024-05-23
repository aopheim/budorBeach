FROM mcr.microsoft.com/dotnet/aspnet:8.0.2-bookworm-slim-arm64v8 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0.201-bookworm-slim-arm64v8 AS build
WORKDIR /src
COPY ["rpiDaemon/rpiDaemon.csproj", "rpiDaemon/"]
COPY ["nuget.config", "rpiDaemon/"]
COPY ["CameraService/CameraService.csproj", "CameraService/"]
COPY ["nuget.config", "CameraService/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["DataAccess.EFCore/DataAccess.EFCore.csproj", "DataAccess.EFCore/"]
COPY ["AudioService/AudioService.csproj", "AudioService/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["Dtos/Dtos.csproj", "Dtos/"]
RUN dotnet restore "rpiDaemon/rpiDaemon.csproj" -r linux-arm64 --configfile "rpiDaemon/nuget.config"
COPY . .
WORKDIR "/src/rpiDaemon"
RUN dotnet build "rpiDaemon.csproj" -c Release -o /app/build -r linux-arm64

FROM build AS publish
RUN dotnet publish "rpiDaemon.csproj" -c Release -o /app/publish -r linux-arm64 --self-contained

FROM base AS final
RUN cd .. && mkdir audioRecordings && mkdir images
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "rpiDaemon.dll"]
