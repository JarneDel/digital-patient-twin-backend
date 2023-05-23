using System.Text.Json.Serialization;

namespace DataGenerator.models;

public class Message : SensorData
{
    [JsonPropertyName("deviceId")]
    public string? DeviceId { get; set; }
}

public class SensorData
{
    [JsonPropertyName("bloeddruk")]
    public BloodPressure Bloeddruk { get; set; } = new BloodPressure();

    [JsonPropertyName("ademFrequentie")]
    public SensorValue AdemFrequentie { get; set; } = new SensorValue();

    [JsonPropertyName("hartslag")]
    public SensorValue Hartslag { get; set; } = new SensorValue();

    [JsonPropertyName("bloedzuurstof")]
    public SensorValue Bloedzuurstof { get; set; } = new SensorValue();

    [JsonPropertyName("temperatuur")]
    public SensorValue Temperatuur { get; set; } = new SensorValue();
}


public class BloodPressure
{
    [JsonPropertyName("systolic")]
    public int Systolic { get; set; }

    [JsonPropertyName("diastolic")]
    public int Diastolic { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
}

public class SensorValue
{
    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
}
