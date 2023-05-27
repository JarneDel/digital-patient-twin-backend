## Rider run

RIDER: per service:
service/Properties/launchSettings.json > dapr > run drukken
Debug: ctrl alt 5 // debugger: attach to process
service selecteren

## Dapr CLI

```bash
cd DataGenerator
dapr run -a DataGenerator -G 60002 -p 5012 --resources-path ../components/ dotnet run
```

```bash
cd PatientData
dapr run -a PatientDataService -H 5010 -p 5000 -G 60000 --resources-path ../components/ dotnet run

```

```bash
cd PatientGegevensService
dapr run -a PricingService -H 5012 -p 5002 -G 60002 --resources-path ../components/ dotnet run
```

Verifying the services are running:

```bash
dapr list
```