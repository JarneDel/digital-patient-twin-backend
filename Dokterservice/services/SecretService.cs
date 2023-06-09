using Dapr.Client;
using Dokterservice.modes;

namespace Dokterservice.services;



public class SecretService : ISecretService
{
    private readonly DaprClient _daprClient;
    public SecretService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    public async Task<string> GetSecret(string secretName)
    {
        Console.WriteLine($"Getting secret {secretName}");
        var secrets = await _daprClient.GetSecretAsync("daprsecrets", secretName);
        var secret = secrets[secretName];
        Console.WriteLine($"Got secret {secretName}");
        return secret;
        
    }

    public async Task<CosmosSecrets> GetCosmosSecrets()
    {
        var cosmosDbConnectionString = GetSecret("CosmosDbConnectionString");
        var databaseName = GetSecret("databaseName");
        var containerName = GetSecret("DokterContainerName");
        await Task.WhenAll(cosmosDbConnectionString, databaseName, containerName);
        return new CosmosSecrets
        {
            ConnectionString = cosmosDbConnectionString.Result,
            DatabaseName = databaseName.Result,
            ContainerName = containerName.Result
        };
    }
}

public interface ISecretService
{
    Task<string> GetSecret(string secretName);
    Task<CosmosSecrets> GetCosmosSecrets();
}