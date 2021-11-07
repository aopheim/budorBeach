FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["budorWeb.Api/budorWeb.Api.csproj", "budorWeb.Api/"]
RUN dotnet restore "budorWeb.Api/budorWeb.Api.csproj"
COPY . .
WORKDIR "/src/budorWeb.Api"
RUN dotnet build "budorWeb.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "budorWeb.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "budorWeb.Api.dll"]
