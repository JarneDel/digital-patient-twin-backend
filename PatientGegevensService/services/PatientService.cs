using PatientGegevensService.models;
using PatientGegevensService.repositories;

namespace PatientGegevensService.services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<List<PatientGegevens>> GetAllPatients() => await _patientRepository.GetAllPatients();
    public async Task<string> AddPatient(PatientGegevens gegevens) => await _patientRepository.AddPatient(gegevens);
    public async Task<PatientGegevens> GetPatient(string id) => await _patientRepository.GetPatient(id);
    public async Task<PatientGegevens> UpdatePatient(PatientGegevens gegevens) => await _patientRepository.UpdatePatient(gegevens);
}

public interface IPatientService
{
    Task<List<PatientGegevens>> GetAllPatients();
    Task<string> AddPatient(PatientGegevens gegevens);
    Task<PatientGegevens> GetPatient(string id);
    Task<PatientGegevens> UpdatePatient(PatientGegevens gegevens);
}