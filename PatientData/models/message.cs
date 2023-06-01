using System.Text.Json.Serialization;

namespace PatientData.models;

public class Message
{
    [JsonPropertyName("deviceId")
    ] public string DeviceId { get; set; }
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
    [JsonPropertyName("bloeddruk")] public BloodPressureHistory Bloeddruk { get; set; } = new();
    [JsonPropertyName("ademFrequentie")] public SensorHistory AdemFrequentie { get; set; } = new();
    [JsonPropertyName("hartslag")] public SensorHistory Hartslag { get; set; } = new();
    [JsonPropertyName("bloedzuurstof")] public SensorHistory Bloedzuurstof { get; set; } = new();
    [JsonPropertyName("temperatuur")] public SensorHistory Temperatuur { get; set; } = new();
}

public class BloodPressureHistory
{
    [JsonPropertyName("systolic")] public SensorHistory Systolic { get; set; } = new();
    [JsonPropertyName("diastolic")] public SensorHistory Diastolic { get; set; } = new();
}

public class SensorHistory
{
    [JsonPropertyName("min")] public decimal Min { get; set; }
    [JsonPropertyName("max")] public decimal Max { get; set; }
    [JsonPropertyName("q1")] public decimal Q1 { get; set; }
    [JsonPropertyName("q3")] public decimal Q3 { get; set; }
    [JsonPropertyName("avg")] public decimal Avg { get; set; }
    
    // [JsonPropertyName("unit")] public string Unit { get; set; } = "";
}