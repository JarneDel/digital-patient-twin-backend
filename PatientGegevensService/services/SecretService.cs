using Dapr.Client;

namespace PatientGegevensService.services;

public class SecretService : ISecretService
{
    private readonly DaprClient _daprClient;
    public SecretService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    public string GetSecret(string secretName)
    {
        return _daprClient.GetSecretAsync("azure-cosmos-db-secrets", secretName).Result[secretName];
    }

    public string GetCosmosDbConnectionString() => GetSecret("connectionString");

    public string GetDatabaseName() => GetSecret("databaseName");

    public string GetContainerName() => GetSecret("PatientGegevensContainerName");
}

public interface ISecretService
{
    string GetSecret(string secretName);
    string GetCosmosDbConnectionString();
    string GetDatabaseName();
    string GetContainerName();
}