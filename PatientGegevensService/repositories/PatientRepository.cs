using System.Net;
using Microsoft.Azure.Cosmos;
using PatientGegevensService.models;
using PatientGegevensService.services;

namespace PatientGegevensService.repositories;

public class PatientRepository: IPatientRepository
{
    private readonly Container _container;

    public PatientRepository(ISecretService secretService)
    {
        var options = new CosmosClientOptions()
        {
            ConnectionMode = ConnectionMode.Gateway
        };
        var connectionStringPromise = secretService.GetCosmosDbConnectionString();
        var containerNamePromise = secretService.GetContainerName();
        var databaseNamePromise = secretService.GetDatabaseName();
        Task.WhenAll(connectionStringPromise, containerNamePromise, databaseNamePromise).GetAwaiter().GetResult();
        var connectionString = connectionStringPromise.Result;
        var containerName = containerNamePromise.Result;
        var databaseName = databaseNamePromise.Result;
        var cosmosClient = new CosmosClient(connectionString, options);
        cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName).Wait();
        cosmosClient.GetDatabase(databaseName).CreateContainerIfNotExistsAsync(containerName, "/id").Wait();
        _container = cosmosClient.GetContainer(databaseName, containerName);
    }

    public async Task<string> AddPatient(PatientGegevens gegevens)
    {
        gegevens.Id ??= Guid.NewGuid().ToString();
        var res = await _container.CreateItemAsync<PatientGegevens>(gegevens, new PartitionKey(gegevens.Id));
        if (res.StatusCode == HttpStatusCode.Created)
        {
            return res.Resource.Id;
        }
        throw new Exception("Something went wrong");
    }

    public async Task<PatientGegevens> GetPatient(string id)
    {
        var res = await _container.ReadItemAsync<PatientGegevens>(id, new PartitionKey(id));
        if (res.StatusCode == HttpStatusCode.OK)
        {
            return res.Resource;
        }
        throw new Exception("Something went wrong");
    }

    public async Task<PatientGegevens> UpdatePatient(PatientGegevens gegevens)
    {
        var res = await _container.UpsertItemAsync<PatientGegevens>(gegevens, new PartitionKey(gegevens.Id));
        if (res.StatusCode == HttpStatusCode.OK)
        {
            return res.Resource;
        }
        throw new Exception("Something went wrong");
    }
}

public interface IPatientRepository
{
    Task<string> AddPatient(PatientGegevens gegevens);
    Task<PatientGegevens> GetPatient(string id);
    Task<PatientGegevens> UpdatePatient(PatientGegevens gegevens);
}