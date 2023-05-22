## Rider run

RIDER: per service:
service/Properties/launchSettings.json > dapr > run drukken
Debug: ctrl alt 5 // debugger: attach to process
service selecteren

## Dapr CLI

```bash
cd AdminService
dapr run -a BasketService -H 5013 -p 5003 -G 60003 --resources-path ../components/ dotnet run
```

```bash
cd RealtimeDataService
dapr run -a CatalogService -H 5010 -p 5001 -G 60001 --resources-path ../components/ dotnet run

```

```bash
cd PatientData
dapr run -a PricingService -H 5012 -p 5002 -G 60002 --resources-path ../components/ dotnet run
```

Verifying the services are running:

```bash
dapr list
```