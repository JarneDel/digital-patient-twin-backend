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
        cosmosClient.GetDatabase(cosmosSecrets.DatabaseName)
            .CreateContainerIfNotExistsAsync(cosmosSecrets.ContainerName, "/patientId").Wait();
        _container = cosmosClient.GetContainer(cosmosSecrets.DatabaseName, cosmosSecrets.ContainerName);
    }

    public async Task<List<Notification>> GetMeldingenById(string patientId, int offset,
        Notification.NotificationLevel level, Notification.NotificationType type)

    {
        QueryDefinition query;
        const string baseQuery = "SELECT * FROM c WHERE c.patientId = @patientId ";
        var levelClause = (level != Notification.NotificationLevel.All) ? "AND c.level = @level " : "";
        var typeClause = (type != Notification.NotificationType.All) ? "AND c.type = @type " : "";
        const string offsetClause = "ORDER BY c._ts DESC OFFSET @offset LIMIT @limit ";
        var queryText = baseQuery + levelClause + typeClause + offsetClause;
        
        query = new QueryDefinition(queryText);
        if (level != Notification.NotificationLevel.All)
            query.WithParameter("@level", level);
        if (type != Notification.NotificationType.All)
            query.WithParameter("@type", type);

        query.WithParameter("@patientId", patientId);
        query.WithParameter("@offset", offset);
        query.WithParameter("@limit", 25);
        var iterator = _container.GetItemQueryIterator<Notification>(query);
        var notifications = new List<Notification>();
        while (iterator.HasMoreResults)
        {
            notifications.AddRange((await iterator.ReadNextAsync()).ToList());
        }

        return notifications;
    }

    public async Task<List<Notification>> GetMeldingenByListId(List<string> patientIds, int offset,
        Notification.NotificationLevel level, Notification.NotificationType type)
    {
        const string baseQuery = "SELECT * FROM c WHERE ARRAY_CONTAINS(@patientIds, c.patientId) ";
        string levelClause = (level != Notification.NotificationLevel.All) ? "AND c.level = @level " : "";
        string typeClause = (type != Notification.NotificationType.All) ? "AND c.type = @type " : "";
        const string orderByClause = "ORDER BY c._ts DESC ";
        const string offsetLimitClause = "OFFSET @offset LIMIT @limit ";
        string completeQuery = $"{baseQuery} {levelClause} {typeClause} {orderByClause} {offsetLimitClause}";
        var query = new QueryDefinition(completeQuery);

        if (level != Notification.NotificationLevel.All)
            query.WithParameter("@level", level);
        if (type != Notification.NotificationType.All)
            query.WithParameter("@type", type);
        
        query.WithParameter("@patientIds", patientIds);
        query.WithParameter("@offset", offset);
        query.WithParameter("@limit", 25);
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
    Task<List<Notification>> GetMeldingenById(string patientId, int offset, Notification.NotificationLevel level,
        Notification.NotificationType type);

    Task<List<Notification>> GetMeldingenByListId(List<string> patientIds, int offset,
        Notification.NotificationLevel level, Notification.NotificationType type);
}