using meldingenservice.Models;
using meldingenservice.services;
using Microsoft.Azure.Cosmos;

namespace meldingenservice.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly Container _container; 
    public NotificationRepository(ISecretService secretService)
    {
        var cosmosSecrets = secretService.GetCosmosSecrets().Result;
        var cosmosOptions = new CosmosClientOptions()
        {
            ConnectionMode = ConnectionMode.Gateway
        };
        
        var cosmosClient = new CosmosClient(cosmosSecrets.ConnectionString, cosmosOptions);
        cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosSecrets.DatabaseName).Wait();
        cosmosClient.GetDatabase(cosmosSecrets.DatabaseName).CreateContainerIfNotExistsAsync(cosmosSecrets.ContainerName, "/id").Wait();
        _container = cosmosClient.GetContainer(cosmosSecrets.DatabaseName, cosmosSecrets.ContainerName);
    }
    
    public async Task<List<Notification>> GetNotificationsById(string id, int offset)
    {
        var query = new QueryDefinition("SELECT TOP @limit * FROM c WHERE c.patientId = @patientId OFFSET @offset")
            .WithParameter("@patientId", id)
            .WithParameter("@offset", offset)
            .WithParameter("@limit", 25);
        var iterator = _container.GetItemQueryIterator<Notification>(query);
        var notifications = new List<Notification>();
        while (iterator.HasMoreResults)
        {
            notifications.AddRange((await iterator.ReadNextAsync()).ToList());
        }
        return notifications;


    }

    public async Task<List<Notification>> GetNotificationByListId(List<string> ids, int offset)
    {
        var query = new QueryDefinition("SELECT TOP @limit * FROM c WHERE ARRAY_CONTAINS(@patientIds, c.patientId) OFFSET @offset" )
            .WithParameter("@patientIds", ids)
            .WithParameter("@offset", offset)
            .WithParameter("@limit", 25);
        var iterator = _container.GetItemQueryIterator<Notification>(query);
        var notifications = new List<Notification>();
        while (iterator.HasMoreResults)
        {
            notifications.AddRange((await iterator.ReadNextAsync()).ToList());
        }
        return notifications;
    }
}

public interface INotificationRepository
{
    Task<List<Notification>> GetNotificationsById(string id, int start, int end);
    Task<List<Notification>> GetNotificationByListId(List<string> ids);
}