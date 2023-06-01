using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace PatientData.models;

public class CosmosEntry
{
    [JsonProperty(PropertyName = "id")] public string Id { get; set; }
    [JsonProperty(PropertyName = "deviceId")] public string DeviceId { get; set; }
    [JsonProperty(PropertyName = "_ts")] public long Timestamp { get; set; }
    [JsonProperty(PropertyName = "bloeddruk")] public BloodPressureValue Bloeddruk { get; set; } = new();
    [JsonProperty(PropertyName = "ademFrequentie")] public SensorValue AdemFrequentie { get; set; } = new();
    [JsonProperty(PropertyName = "hartslag")] public SensorValue Hartslag { get; set; } = new();
    [JsonProperty(PropertyName = "bloedzuurstof")] public SensorValue Bloedzuurstof { get; set; } = new();
    [JsonProperty(PropertyName = "temperatuur")] public SensorValue Temperatuur { get; set; } = new();
}

public class BloodPressureValue
{
    [JsonProperty(PropertyName = "systolic")] public decimal Systolic { get; set; }
    [JsonProperty(PropertyName = "diastolic")] public decimal Diastolic { get; set; }
    [JsonProperty(PropertyName = "unit")] public string Unit { get; set; } = "";
}

public class SensorValue
{
    [JsonProperty(PropertyName = "value")] public decimal Value { get; set; }
    [JsonProperty(PropertyName = "unit")] public string Unit { get; set; } = "";
}
