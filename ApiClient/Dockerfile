FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ApiClient/ApiClient.csproj", "ApiClient/"]
RUN dotnet restore "ApiClient/ApiClient.csproj"
COPY . .
WORKDIR "/src/ApiClient"
RUN dotnet build "ApiClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiClient.dll"]
