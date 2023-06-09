namespace PatientData.models;



public static class ConversionHelper
{
    public static VitalStats ConvertToVitalStats(Stats stats)
    {
        var vitalStats = new VitalStats
        {
            Systolic = new VitalStats.VitalStat
            {
                Min = stats.MinSystolic,
                Avg = stats.AvgSystolic,
                Max = stats.MaxSystolic,
                Unit = "mmHg"
            },
            Diastolic = new VitalStats.VitalStat
            {
                Min = stats.MinDiastolic,
                Avg = stats.AvgDiastolic,
                Max = stats.MaxDiastolic,
                Unit = "mmHg"
            },
            AdemFrequentie = new VitalStats.VitalStat
            {
                Min = stats.MinAdemFrequentie,
                Avg = stats.AvgAdemFrequentie,
                Max = stats.MaxAdemFrequentie,
                Unit = "/min"
            },
            Hartslag = new VitalStats.VitalStat
            {
                Min = stats.MinHartslag,
                Avg = stats.AvgHartslag,
                Max = stats.MaxHartslag,
                Unit = "bpm"
            },
            Bloedzuurstof = new VitalStats.VitalStat
            {
                Min = stats.MinBloedzuurstof,
                Avg = stats.AvgBloedzuurstof,
                Max = stats.MaxBloedzuurstof,
                Unit = "%"
            },
            Temperatuur = new VitalStats.VitalStat
            {
                Min = Convert.ToInt32(stats.MinTemperatuur),
                Avg = stats.AvgTemperatuur,
                Max = Convert.ToInt32(stats.MaxTemperatuur),
                Unit = "°C"
            }
        };

        return vitalStats;
    }
}