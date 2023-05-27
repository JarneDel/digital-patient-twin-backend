using Microsoft.Azure.Cosmos;
using PatientData.models;
using PatientData.services;

namespace PatientData.repositories;

public class HistoriekRepository : IHistoriekRepository
{
    
    private readonly ISecretService _secretService;
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;
    
    
    public HistoriekRepository(ISecretService secretService)
    {
        _secretService = secretService;
        var options = new CosmosClientOptions(){
            ConnectionMode = ConnectionMode.Gateway
        };
        _cosmosClient = new CosmosClient(_secretService.GetCosmosDbConnectionString(), options);
        _container = GetContainerAsync().Result;
    }
    
    private async Task<Container> GetContainerAsync()
    {
        var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_secretService.GetDatabaseName());
        return await database.Database.CreateContainerIfNotExistsAsync(_secretService.GetContainerName(), "/deviceId");
    }

    public async Task<List<CosmosEntry>> ReadRange(DateTime start, DateTime end, string deviceId)
    {

        var startUnix = new DateTimeOffset(start).ToUnixTimeSeconds();
        var endUnix = new DateTimeOffset(end).ToUnixTimeSeconds();

        var query = new QueryDefinition("SELECT c._ts, c.temperatuur, c.bloeddruk, c.ademFrequentie, c.hartslag, c.bloedzuurstof, c.deviceId, c.id FROM c WHERE c.deviceId = @deviceId AND c._ts >= @start AND c._ts <= @end")
            .WithParameter("@deviceId", deviceId)
            .WithParameter("@start", startUnix)
            .WithParameter("@end", endUnix);

        var iterator = _container.GetItemQueryIterator<CosmosEntry>(query);
        var results = new List<CosmosEntry>();
        while (iterator.HasMoreResults)
        {
            var result = await iterator.ReadNextAsync();
            results.AddRange(result);
        }
        return results;

    }

    public async Task<Stats> ReadStats(DateTime start, DateTime end, string deviceId)
    {
        var startUnix = new DateTimeOffset(start).ToUnixTimeSeconds();
        var endUnix = new DateTimeOffset(end).ToUnixTimeSeconds();
        // get avg, min, max, median, mode, std dev
        var query = new QueryDefinition(@"SELECT 
            MIN(c.bloeddruk['systolic']) AS minSystolic, 
            AVG(c.bloeddruk['systolic']) AS avgSystolic, 
            MAX(c.bloeddruk['systolic']) AS maxSystolic, 
            MIN(c.bloeddruk['diastolic']) AS minDiastolic, 
            AVG(c.bloeddruk['diastolic']) AS avgDiastolic, 
            MAX(c.bloeddruk['diastolic']) AS maxDiastolic, 
            MIN(c.ademFrequentie['value']) AS minAdemFrequentie, 
            AVG(c.ademFrequentie['value']) AS avgAdemFrequentie, 
            MAX(c.ademFrequentie['value']) AS maxAdemFrequentie, 
            MIN(c.hartslag['value']) AS minHartslag, 
            AVG(c.hartslag['value']) AS avgHartslag, 
            MAX(c.hartslag['value']) AS maxHartslag, 
            MIN(c.bloedzuurstof['value']) AS minBloedzuurstof, 
            AVG(c.bloedzuurstof['value']) AS avgBloedzuurstof, 
            MAX(c.bloedzuurstof['value']) AS maxBloedzuurstof, 
            MIN(c.temperatuur['value']) AS minTemperatuur, 
            AVG(c.temperatuur['value']) AS avgTemperatuur, 
            MAX(c.temperatuur['value']) AS maxTemperatuur
        FROM c WHERE c.deviceId = @deviceId AND c._ts >= @start AND c._ts <= @end")
            .WithParameter("@deviceId", deviceId)
            .WithParameter("@start", startUnix)
            .WithParameter("@end", endUnix);
        var iterator = _container.GetItemQueryIterator<Stats>(query);
        var results = new List<Stats>();
        while (iterator.HasMoreResults)
        {
            var result = await iterator.ReadNextAsync();
            results.AddRange(result);
        }
        var stats = results.First();
        return stats;

    }



}

public interface IHistoriekRepository
{
    Task<List<CosmosEntry>> ReadRange(DateTime start, DateTime end, string deviceId);

}