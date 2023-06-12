using Newtonsoft.Json;

namespace Dokterservice.modes;

public class NotificationSettings
{
    public class Notification
    {
        [JsonProperty(PropertyName = "isEnabled")]
        public bool IsEnabled { get; set; }
        [JsonProperty(PropertyName = "thresholdMin")]
        public int ThresholdMin { get; set; }
        [JsonProperty(PropertyName = "thresholdMax")]
        public int ThresholdMax { get; set; }
    }
    [JsonProperty(PropertyName = "patientId")]
    public string PatientId { get; set; }
    [JsonProperty(PropertyName = "bloeddrukSystolic")]
    public Notification BloeddrukSystolic { get; set; }
    [JsonProperty(PropertyName = "bloeddrukDiastolic")]
    public Notification BloeddrukDiastolic { get; set; }
    [JsonProperty(PropertyName = "hartslag")]
    public Notification Hartslag { get; set; }
    [JsonProperty(PropertyName = "ademFrequentie")]
    public Notification AdemFrequentie { get; set; }
    [JsonProperty(PropertyName = "temperatuur")]
    public Notification Temperatuur { get; set; }
    [JsonProperty(PropertyName = "zuurstofSaturatie")]
    public Notification ZuurstofSaturatie { get; set; }
    
}