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
        Console.WriteLine("getting secret: " + secretName);
        var secret =  _daprClient.GetSecretAsync("daprsecrets", secretName).Result[secretName];
        Console.WriteLine("Got secret:" + secretName);
        return secret;
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