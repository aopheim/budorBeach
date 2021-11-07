FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TSGenerator/TSGenerator.csproj", "TSGenerator/"]
RUN dotnet restore "TSGenerator/TSGenerator.csproj"
COPY . .
WORKDIR "/src/TSGenerator"
RUN dotnet build "TSGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TSGenerator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TSGenerator.dll"]
