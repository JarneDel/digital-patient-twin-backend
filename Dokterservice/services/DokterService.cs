using Dapr.Client;
using Dokterservice.models;
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

    public async Task<EnabledNotifications> CreateOrUpdateNotificationSettingsByPatient(string id, string patientId,
        EnabledNotifications enabledNotifications)
    {
        return await _dokterRepository.CreateOrUpdateNotificationSettingsByPatient(id, patientId, enabledNotifications);
    }

    public async Task<EnabledNotifications> GetNotificationSettingsByPatient(string id, string patientId)
    {
        return await _dokterRepository.GetNotificationSettingsByPatient(id, patientId);
    }

    public async Task<Dokter> PinPatientToDokter(string id, string patientId)
    {
        return await _dokterRepository.PinPatientToDokter(id, patientId);
    }

    public async Task<Dokter> UnpinPatientFromDokter(string id, string patientId)
    {
        return await _dokterRepository.UnpinPatientFromDokter(id, patientId);
    }

    public async Task<List<PatientGegevens>> GetPatientsByDokter(string id)
    {
        // get dokter by id
        var dokter = await _dokterRepository.GetDokter(id);
        // get all patients
        var daprRequest = _daprClient.CreateInvokeMethodRequest(HttpMethod.Post, "patientgegevensservice",
            "patient/multiple", dokter.PatientIds);
        var response = await _daprClient.InvokeMethodAsync<List<PatientGegevens>>(daprRequest);
        return response;
    }

    public async Task<List<PatientGegevens>> GetPinnedPatientsByDokter(string id)
    {
        // get dokter by id
        var dokter = await _dokterRepository.GetDokter(id);
        // get all patients
        if (dokter.PinnedPatients == null)
        {
            return new List<PatientGegevens>();
        }
        var daprRequest = _daprClient.CreateInvokeMethodRequest(HttpMethod.Post, "patientgegevensservice",
            "patient/multiple", dokter.PinnedPatients);
        
        return await _daprClient.InvokeMethodAsync<List<PatientGegevens>>(daprRequest);
    }
}

public interface IDokterService
{
    Task<Dokter> GetDokter(string id);
    Task<Dokter> AddPatientToDokter(string id, string patientId);
    Task<Dokter> RemovePatientFromDokter(string id, string patientId);

    Task<EnabledNotifications> CreateOrUpdateNotificationSettingsByPatient(string id, string patientId,
        EnabledNotifications enabledNotifications);

    Task<EnabledNotifications> GetNotificationSettingsByPatient(string id, string patientId);

    Task<Dokter> PinPatientToDokter(string id, string patientId);
    Task<Dokter> UnpinPatientFromDokter(string id, string patientId);

    Task<List<PatientGegevens>> GetPatientsByDokter(string id);
    Task<List<PatientGegevens>> GetPinnedPatientsByDokter(string id);
}