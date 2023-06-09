using Dapr.Client;
using Dokterservice.modes;
using Dokterservice.repositories;

namespace Dokterservice.services;

public class DokterService : IDokterService
{
    private readonly IDokterRepository _dokterRepository;
    private readonly DaprClient _daprClient;

    public DokterService(IDokterRepository dokterRepository, DaprClient daprClient)
    {
        _dokterRepository = dokterRepository;
        _daprClient = daprClient;
    }

    private async Task<bool> ValidateIfPatientExists(string patientId)
    {
        try
        {
            const string patientGegevensService = "patientgegevensservice";
            var invokeRequest = _daprClient.CreateInvokeMethodRequest<string>(HttpMethod.Get, patientGegevensService,
                "patient/" + patientId, null);
            var response = await _daprClient.InvokeMethodAsync<dynamic>(invokeRequest);
            return response != null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }


    public async Task<Dokter> GetDokter(string id)
    {
        return await _dokterRepository.GetDokter(id);
    }

    public async Task<Dokter> AddPatientToDokter(string id, string patientId)
    {
        // TODO: Check if patient exists
        if (await ValidateIfPatientExists(id))
        {
            throw new ArgumentException("Patient does not exist");
        }

        return await _dokterRepository.AddPatientToDokter(id, patientId);
    }

    public async Task<Dokter> RemovePatientFromDokter(string id, string patientId)
    {
        return await _dokterRepository.RemovePatientFromDokter(id, patientId);
    }

    public async Task<NotificationSettings> CreateOrUpdateNotificationSettingsByPatient(string id, string patientId,
        NotificationSettings notificationSettings)
    {
        return await _dokterRepository.CreateOrUpdateNotificationSettingsByPatient(id, patientId, notificationSettings);
    }

    public async Task<NotificationSettings> GetNotificationSettingsByPatient(string id, string patientId)
    {
        return await _dokterRepository.GetNotificationSettingsByPatient(id, patientId);
    }
}

public interface IDokterService
{
    Task<Dokter> GetDokter(string id);
    Task<Dokter> AddPatientToDokter(string id, string patientId);
    Task<Dokter> RemovePatientFromDokter(string id, string patientId);

    Task<NotificationSettings> CreateOrUpdateNotificationSettingsByPatient(string id, string patientId,
        NotificationSettings notificationSettings);

    Task<NotificationSettings> GetNotificationSettingsByPatient(string id, string patientId);
}