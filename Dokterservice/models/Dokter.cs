using Newtonsoft.Json;

namespace Dokterservice.modes;

public class Dokter
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "notificationSettings")]
    public List<NotificationSettings> NotificationSettings { get; set; }
    [JsonProperty(PropertyName = "patientIds")]
    public List<string> PatientIds { get; set; }
    [JsonProperty(PropertyName = "pinnedPatients")]
    public List<string> PinnedPatients { get; set; }
}