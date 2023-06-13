using Newtonsoft.Json;

namespace meldingenservice.Models;

public class Dokter
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "notificationSettings")]
    public List<EnabledNotifications> NotificationSettings { get; set; }
    [JsonProperty(PropertyName = "patientIds")]
    public List<string> PatientIds { get; set; }
    [JsonProperty(PropertyName = "pinnedPatients")]
    public List<string> PinnedPatients { get; set; }
}

public class EnabledNotifications
{
    [JsonProperty("patientId")]
    public string PatientId { get; set; }

    [JsonProperty("masterSwitch")]
    public bool MasterSwitch { get; set; }

    [JsonProperty("bloeddruk")]
    public bool Bloeddruk { get; set; }

    [JsonProperty("hartslag")]
    public bool Hartslag { get; set; }

    [JsonProperty("ademhalingsfrequentie")]
    public bool Ademhalingsfrequentie { get; set; }

    [JsonProperty("temperatuur")]
    public bool Temperatuur { get; set; }

    [JsonProperty("bloedzuurstof")]
    public bool Bloedzuurstof { get; set; }
}

