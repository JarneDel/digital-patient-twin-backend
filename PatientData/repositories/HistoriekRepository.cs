using Microsoft.Azure.Cosmos;
using PatientData.models;
using PatientData.services;

namespace PatientData.repositories;

public class HistoriekRepository : IHistoriekRepository
{
    
    private readonly ISecretService _secretService;
    private readonly CosmosClient _cosmosClient;
    
    
    public HistoriekRepository(ISecretService secretService)
    {
        _secretService = secretService;
        var options = new CosmosClientOptions(){
            ConnectionMode = ConnectionMode.Gateway
        };
        _cosmosClient = new CosmosClient(_secretService.GetCosmosDbConnectionString(), options);
    }
    
    private async Task<Container> GetContainerAsync()
    {
        var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_secretService.GetDatabaseName());
        return await database.Database.CreateContainerIfNotExistsAsync(_secretService.GetContainerName(), "/deviceId");
    }

    public async Task<List<CosmosEntry>> ReadRange(DateTime start, DateTime end, string deviceId)
    {
        var container = await GetContainerAsync();
        var startUnix = new DateTimeOffset(start).ToUnixTimeSeconds();
        var endUnix = new DateTimeOffset(end).ToUnixTimeSeconds();

        var query = new QueryDefinition("SELECT c._ts, c.temperatuur, c.bloeddruk, c.ademFrequentie, c.hartslag, c.bloedzuurstof, c.deviceId, c.id FROM c WHERE c.deviceId = @deviceId AND c._ts >= @start AND c._ts <= @end")
            .WithParameter("@deviceId", deviceId)
            .WithParameter("@start", startUnix)
            .WithParameter("@end", endUnix);

        var iterator = container.GetItemQueryIterator<CosmosEntry>(query);
        var results = new List<CosmosEntry>();
        while (iterator.HasMoreResults)
        {
            var result = await iterator.ReadNextAsync();
            results.AddRange(result);
        }
        return results;

    }
    
    
}

public interface IHistoriekRepository
{
    Task<List<CosmosEntry>> ReadRange(DateTime start, DateTime end, string deviceId);

}