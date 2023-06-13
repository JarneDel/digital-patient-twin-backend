using System.Net;
using Dokterservice.models;
using Dokterservice.modes;
using Dokterservice.services;
using Microsoft.Azure.Cosmos;

namespace Dokterservice.repositories;

public class DokterRepository : IDokterRepository
{
    private readonly Container _container;

    public DokterRepository(ISecretService secretService)
    {
        var options = new CosmosClientOptions()
        {
            ConnectionMode = ConnectionMode.Gateway
        };
        var cosmosSecrets = secretService.GetCosmosSecrets().Result;
        var cosmosClient = new CosmosClient(cosmosSecrets.ConnectionString, options);
        cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosSecrets.DatabaseName).Wait();
        cosmosClient.GetDatabase(cosmosSecrets.DatabaseName).CreateContainerIfNotExistsAsync(cosmosSecrets.ContainerName, "/id").Wait();
        _container = cosmosClient.GetContainer(cosmosSecrets.DatabaseName, cosmosSecrets.ContainerName);
    }


    public async Task<Dokter> GetDokter(string id)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
            .WithParameter("@id", id);
        var iterator = _container.GetItemQueryIterator<Dokter>(query);
        var dokterResponse = await iterator.ReadNextAsync();
        var dokter = dokterResponse.FirstOrDefault();
        if (dokter != null) return dokter;
        
        // if no dokter is found, create a new one
        dokter = new Dokter()
        {
            Id = id,
            PatientIds = new List<string>(),
            EnabledNotificationsList = new List<EnabledNotifications>()
        };
        var res = await _container.CreateItemAsync<Dokter>(dokter, new PartitionKey(dokter.Id));
        if (res.StatusCode == HttpStatusCode.Created)
        {
            return res.Resource;
        }
        throw new Exception("Something went wrong");

    }

    public async Task<Dokter> AddPatientToDokter(string id, string patientId)
    {
        var dokter = await GetDokter(id);
        
        // check if patient already exists
        var patient = dokter.PatientIds.FirstOrDefault(p => p == patientId);
        if (patient != null)
        {
            return dokter;
        }
        dokter.PatientIds.Add(patientId);
        var res = await _container.UpsertItemAsync<Dokter>(dokter, new PartitionKey(dokter.Id));
        if (res.StatusCode == HttpStatusCode.OK)
        {
            return res.Resource;
        }
        Console.Write(res.StatusCode);
        throw new Exception("Something went wrong");
    }

    public async Task<Dokter> RemovePatientFromDokter(string id, string patientId)
    {
        var dokter = await GetDokter(id);
        var didExist  = dokter.PatientIds.Remove(patientId);
        if (!didExist)
        {
            return dokter;
        }
        var res = await _container.UpsertItemAsync<Dokter>(dokter, new PartitionKey(dokter.Id));
        if (res.StatusCode == HttpStatusCode.OK)
        {
            return res.Resource;
        }
        throw new Exception("Something went wrong");
    }

    public async Task<EnabledNotifications> CreateOrUpdateNotificationSettingsByPatient(string id, string patientId, EnabledNotifications enabledNotifications)
    {
        var dokter = await GetDokter(id);
        var patient = dokter.PatientIds.FirstOrDefault(p => p == patientId);
        if (patient == null)
        {
            throw new Exception("Patient not found");
        }

        dokter.EnabledNotificationsList ??= new List<EnabledNotifications>();
        var settings = dokter.EnabledNotificationsList.FirstOrDefault(s => s.PatientId == patientId);
        enabledNotifications.PatientId ??= patientId;
        dokter.EnabledNotificationsList.Remove(settings);
        dokter.EnabledNotificationsList.Add(enabledNotifications);
        var res = await _container.UpsertItemAsync<Dokter>(dokter, new PartitionKey(dokter.Id));
        if (res.StatusCode == HttpStatusCode.OK)
        {
            return enabledNotifications;
        }
        throw new Exception("Something went wrong");   
    }

    public async Task<EnabledNotifications> GetNotificationSettingsByPatient(string id, string patientId)
    {
        var dokter = await GetDokter(id);
        var patient = dokter.PatientIds.FirstOrDefault(p => p == patientId);
        if (patient == null)
        {
            throw new Exception("Patient not found");
        }
        var settings = dokter.EnabledNotificationsList.FirstOrDefault(s => s.PatientId == patientId);
        if (settings == null)
        {
            throw new Exception("Notification settings not found");
        }
        return settings;
    }

    public async Task<Dokter> PinPatientToDokter(string id, string patientId)
    {
        var dokter = await GetDokter(id);
        var patient = dokter.PatientIds.FirstOrDefault(p => p == patientId);
        if (patient == null) throw new Exception("Patient not found");
        dokter.PinnedPatients ??= new List<string>();
        dokter.PinnedPatients.Add(patientId);
        var res = await _container.UpsertItemAsync<Dokter>(dokter, new PartitionKey(dokter.Id));
        if (res.StatusCode == HttpStatusCode.OK) return res.Resource;
        throw new Exception("Something went wrong");
    }

    public async Task<Dokter> UnpinPatientFromDokter(string id, string patientId)
    {
        var dokter = await GetDokter(id);
        var patient = dokter.PatientIds.FirstOrDefault(p => p == patientId);
        if (patient == null) throw new Exception("Patient not found");
        var didExist = dokter.PinnedPatients.Remove(patientId);
        if (!didExist) return dokter;
        var res = await _container.UpsertItemAsync<Dokter>(dokter, new PartitionKey(dokter.Id));
        if (res.StatusCode == HttpStatusCode.OK) return res.Resource;
        throw new Exception("Something went wrong");
    }
}

public interface IDokterRepository
{
    Task<Dokter> GetDokter(string id);
    Task<Dokter> AddPatientToDokter(string id, string patientId);
    Task<Dokter> RemovePatientFromDokter(string id, string patientId);
    Task<EnabledNotifications> CreateOrUpdateNotificationSettingsByPatient(string id, string patientId, EnabledNotifications enabledNotifications);
    Task<EnabledNotifications> GetNotificationSettingsByPatient(string id, string patientId);
    
    Task<Dokter> PinPatientToDokter(string id, string patientId);
    Task<Dokter> UnpinPatientFromDokter(string id, string patientId);
    
}