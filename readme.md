## Rider run

RIDER: per service:
service/Properties/launchSettings.json > dapr > run drukken
Debug: ctrl alt 5 // debugger: attach to process
service selecteren

## Dapr CLI

```bash
cd DataGenerator
dapr run -a DataGenerator -G 60002 -H 5012 --config --resources-path ../components/ dotnet run
```

```bash
cd PatientData
dapr run -a PatientData -H 5010 -p 5000 -G 60000 --config --resources-path ../components/ dotnet run

```

```bash
cd PatientGegevensService
dapr run -a PatientGegevensService -p 5001 -H 5011 -G 60001 --config --resources-path ../components/ dotnet run
```

Verifying the services are running:

```bash
dapr list
```