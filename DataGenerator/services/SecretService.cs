using Dapr.Client;
using DataGenerator.models;

namespace DataGenerator.services;

public class SecretService : ISecretService
{
    private readonly DaprClient _daprClient;
    public SecretService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    
    public string GetSecret(string secretName)
    {
        var secret = _daprClient.GetSecretAsync("azure-iot-hub-secrets", secretName).Result;
        return secret[secretName];
    }

    public IotHubSecrets GetIoTHubSecrets()
    {
        return new IotHubSecrets
        {
            Server = GetSecret("Server"),
            ClientId = GetSecret("ClientId"),
            UserName = GetSecret("UserName"),
            Password = GetSecret("Password"),
            Port = int.TryParse(GetSecret("Port"), out var port) ? port : 8883
        };
    }
}

public interface ISecretService
{
    string GetSecret(string secretName);
    IotHubSecrets GetIoTHubSecrets();
    
}
