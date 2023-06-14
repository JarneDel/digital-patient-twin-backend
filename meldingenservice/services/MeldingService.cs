using Dapr.Client;
using meldingenservice.Models;
using meldingenservice.Repositories;

namespace meldingenservice.services;

public class MeldingService : IMeldingService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly DaprClient _daprClient;

    public MeldingService(INotificationRepository notificationRepository, DaprClient daprClient)
    {
        _notificationRepository = notificationRepository;
        _daprClient = daprClient;
    }

    public async Task<List<Notification>> GetMeldingenById(string patientId, int offset,
        Notification.NotificationLevel level, Notification.NotificationType type)
    {
        return await _notificationRepository.GetMeldingenById(patientId, offset, level, type);
    }

    public async Task<List<Notification>> GetMeldingenByDoctorId(string doctorId, int offset,
        Notification.NotificationLevel level, Notification.NotificationType type)
    {
        // get all patients from doctor
        var requestMesssage =
            _daprClient.CreateInvokeMethodRequest<string>(HttpMethod.Get, "dokterservice", "dokter/" + doctorId, null);
        Console.WriteLine("Getting Patient with request: " + requestMesssage.RequestUri?.AbsoluteUri);
        var response = await _daprClient.InvokeMethodAsync<Dokter>(requestMesssage);
        var patients = response.PatientIds;
        // get all notifications from patients
        var notifications = await _notificationRepository.GetMeldingenByListId(patients, offset, level, type);
        return notifications;
    }

    public async Task<List<Notification>> GetMeldingenByPatientIdAndDoctorId(string patientId, string doctorId,
        int offset, Notification.NotificationLevel level,
        Notification.NotificationType type)
    {
        var requestMesssage =
            _daprClient.CreateInvokeMethodRequest<string>(HttpMethod.Get, "dokterservice", "dokter/" + doctorId, null);
        var response = await _daprClient.InvokeMethodAsync<Dokter>(requestMesssage);
        var patients = response.PatientIds;
        if (!patients.Contains(patientId))
        {
            throw new PatientNotFoundException("Patient is not in the list of patients of this doctor");
        }

        var notifications = await _notificationRepository.GetMeldingenById(patientId, offset, level, type);
        return notifications;
    }
}

public interface IMeldingService
{
    Task<List<Notification>> GetMeldingenById(string patientId, int offset, Notification.NotificationLevel level,
        Notification.NotificationType type);

    Task<List<Notification>> GetMeldingenByDoctorId(string doctorId, int offset, Notification.NotificationLevel level,
        Notification.NotificationType type);

    Task<List<Notification>> GetMeldingenByPatientIdAndDoctorId(string patientId, string doctorId, int offset,
        Notification.NotificationLevel level,
        Notification.NotificationType type);
}