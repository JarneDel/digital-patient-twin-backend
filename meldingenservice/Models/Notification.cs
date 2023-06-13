using Newtonsoft.Json;

namespace meldingenservice.Models;

public class Notification
{
    public Notification()
    {
        
    }
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "patientId")]
    public string PatientId { get; set; }
    [JsonProperty(PropertyName = "geboortedatum")]
    public string BirthDate { get; set; }
    
    
    [JsonProperty(PropertyName = "name")]
    public string FullName { get; set; }
    [JsonProperty(PropertyName = "type")]
    public NotificationType Type { get; set; }
    public enum NotificationType
    {
        [JsonProperty(PropertyName = "Bloeddruk")]
        BloodPressure,
        [JsonProperty(PropertyName = "Temperatuur")]
        Temperature,
        [JsonProperty(PropertyName = "Hartslag")]
        HeartRate,
        [JsonProperty(PropertyName = "Ademhalingsfrequentie")]
        BreathingRate,
        [JsonProperty(PropertyName = "Bloedzuurstof")]
        BloodOxygen
    }
    
    [JsonProperty(PropertyName = "level")]
    public NotificationLevel Level { get; set; }
    
    public enum NotificationLevel
    {
        [JsonProperty(PropertyName = "Info")]
        Info,
        [JsonProperty(PropertyName = "Waarschuwing")]
        Warning,
        [JsonProperty(PropertyName = "Ernstig")]
        Critical,
        None
    }
    [JsonProperty(PropertyName = "value")]
    public decimal Value { get; set; }
    [JsonProperty(PropertyName = "time")]
    public DateTime Timestamp { get; set; }
    

}