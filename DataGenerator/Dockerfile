FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DataGenerator/DataGenerator.csproj", "DataGenerator/"]
RUN dotnet restore "DataGenerator/DataGenerator.csproj"
COPY . .
WORKDIR "/src/DataGenerator"
RUN dotnet build "DataGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataGenerator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataGenerator.dll"]
