namespace PatientData.models;

using Newtonsoft.Json;

public class Stats
{
    [JsonProperty("minSystolic")]
    public int MinSystolic { get; set; }

    [JsonProperty("avgSystolic")]
    public double AvgSystolic { get; set; }

    [JsonProperty("maxSystolic")]
    public int MaxSystolic { get; set; }

    [JsonProperty("minDiastolic")]
    public int MinDiastolic { get; set; }

    [JsonProperty("avgDiastolic")]
    public double AvgDiastolic { get; set; }

    [JsonProperty("maxDiastolic")]
    public int MaxDiastolic { get; set; }

    [JsonProperty("minAdemFrequentie")]
    public int MinAdemFrequentie { get; set; }

    [JsonProperty("avgAdemFrequentie")]
    public double AvgAdemFrequentie { get; set; }

    [JsonProperty("maxAdemFrequentie")]
    public int MaxAdemFrequentie { get; set; }

    [JsonProperty("minHartslag")]
    public int MinHartslag { get; set; }

    [JsonProperty("avgHartslag")]
    public double AvgHartslag { get; set; }

    [JsonProperty("maxHartslag")]
    public int MaxHartslag { get; set; }

    [JsonProperty("minBloedzuurstof")]
    public int MinBloedzuurstof { get; set; }

    [JsonProperty("avgBloedzuurstof")]
    public double AvgBloedzuurstof { get; set; }

    [JsonProperty("maxBloedzuurstof")]
    public int MaxBloedzuurstof { get; set; }

    [JsonProperty("minTemperatuur")]
    public double MinTemperatuur { get; set; }

    [JsonProperty("avgTemperatuur")]
    public double AvgTemperatuur { get; set; }

    [JsonProperty("maxTemperatuur")]
    public double MaxTemperatuur { get; set; }
}

public class VitalStats
{
    public class VitalStat
    {
        [JsonProperty("min")]
        public int Min { get; set; }
        [JsonProperty("avg")]
        public double Avg { get; set; }
        [JsonProperty("max")]
        public int Max { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; }
    }

    [JsonProperty("systolic")]
    public VitalStat Systolic { get; set; }
    [JsonProperty("diastolic")]
    public VitalStat Diastolic { get; set; }
    [JsonProperty("ademFrequentie")]
    public VitalStat AdemFrequentie { get; set; }
    [JsonProperty("hartslag")]
    public VitalStat Hartslag { get; set; }
    [JsonProperty("bloedzuurstof")]
    public VitalStat Bloedzuurstof { get; set; }
    [JsonProperty("temperatuur")]
    public VitalStat Temperatuur { get; set; }
}