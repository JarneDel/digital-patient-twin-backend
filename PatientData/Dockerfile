FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PatientData/PatientData.csproj", "PatientData/"]
RUN dotnet restore "PatientData/PatientData.csproj"
COPY . .
WORKDIR "/src/PatientData"
RUN dotnet build "PatientData.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PatientData.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PatientData.dll"]
