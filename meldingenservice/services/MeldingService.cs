using meldingenservice.Models;

namespace meldingenservice.services;

public class MeldingService : IMeldingService
{
  public async Task<List<Notification>> GetMeldingenById(string patientId, int start, int end)
  {
    throw new NotImplementedException();
  }

  public async Task<List<Notification>> GetMeldingenByDoctorId(string doctorId, int start, int end)
  {
    throw new NotImplementedException();
  }
}

public interface IMeldingService
{
  Task<List<Notification>> GetMeldingenById(string patientId, int start, int end);  
  Task<List<Notification>> GetMeldingenByDoctorId(string doctorId, int start, int end);
}