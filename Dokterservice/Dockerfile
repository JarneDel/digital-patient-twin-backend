FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Dokterservice/Dokterservice.csproj", "Dokterservice/"]
RUN dotnet restore "Dokterservice/Dokterservice.csproj"
COPY . .
WORKDIR "/src/Dokterservice"
RUN dotnet build "Dokterservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dokterservice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dokterservice.dll"]
