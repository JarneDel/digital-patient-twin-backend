using Dapr.Client;

namespace PatientData.services;

public class SecretService : ISecretService
{
    private readonly DaprClient _daprClient;
    public SecretService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    public string GetSecret(string secretName)
    {
        return _daprClient.GetSecretAsync("daprsecrets", secretName).Result[secretName];
    }

    public string GetCosmosDbConnectionString() => GetSecret("CosmosDbConnectionString");

    public string GetDatabaseName() => GetSecret("databaseName");

    public string GetContainerName() => GetSecret("RealtimeDatacontainerName");
}

public interface ISecretService
{
    string GetSecret(string secretName);
    string GetCosmosDbConnectionString();
    string GetDatabaseName();
    string GetContainerName();
}