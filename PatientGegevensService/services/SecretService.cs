using Dapr.Client;
using Microsoft.AspNetCore.DataProtection;

namespace PatientGegevensService.services;

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
 
    public Task<string> GetCosmosDbConnectionString() => GetSecret("CosmosDbConnectionString");

    public Task<string> GetDatabaseName() => GetSecret("databaseName");

    public Task<string> GetContainerName() => GetSecret("PatientGegevensContainerName");
}

public interface ISecretService
{
    Task<string> GetSecret(string secretName);
    Task<string> GetCosmosDbConnectionString();
    Task<string> GetDatabaseName();
    Task<string> GetContainerName();
}