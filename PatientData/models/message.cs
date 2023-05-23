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
    [JsonPropertyName("systolicMin")] public int SystolicMin { get; set; }

    [JsonPropertyName("systolicMax")] public int SystolicMax { get; set; }

    [JsonPropertyName("systolicAvg")] public int SystolicAvg { get; set; }

    [JsonPropertyName("diastolicMin")] public int DiastolicMin { get; set; }

    [JsonPropertyName("diastolicMax")] public int DiastolicMax { get; set; }

    [JsonPropertyName("diastolicAvg")] public int DiastolicAvg { get; set; }
}

public class SensorHistory
{
    [JsonPropertyName("min")] public decimal Min { get; set; }
    [JsonPropertyName("max")] public decimal Max { get; set; }
    [JsonPropertyName("avg")] public decimal Avg { get; set; }
    [JsonPropertyName("unit")] public string Unit { get; set; } = "";
}