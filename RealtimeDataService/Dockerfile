FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RealtimeDataService/RealtimeDataService.csproj", "RealtimeDataService/"]
RUN dotnet restore "RealtimeDataService/RealtimeDataService.csproj"
COPY . .
WORKDIR "/src/RealtimeDataService"
RUN dotnet build "RealtimeDataService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealtimeDataService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealtimeDataService.dll"]
