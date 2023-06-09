using Dokterservice.modes;
using Dokterservice.repositories;

namespace Dokterservice.services;

public class DokterService : IDokterService
{
    private readonly IDokterRepository _dokterRepository;
    public DokterService(IDokterRepository dokterRepository)
    {
        _dokterRepository = dokterRepository;
        
    }
    
    
    public async Task<Dokter> GetDokter(string id)
    {
        return await _dokterRepository.GetDokter(id);
    }

    public async Task<Dokter> AddPatientToDokter(string id, string patientId)
    {
        // TODO: Check if patient exists
        
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
    Task<NotificationSettings> CreateOrUpdateNotificationSettingsByPatient(string id, string patientId, NotificationSettings notificationSettings);
    Task<NotificationSettings> GetNotificationSettingsByPatient(string id, string patientId);
}