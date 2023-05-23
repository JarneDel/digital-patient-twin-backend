using PatientData.models;
using PatientData.repositories;

namespace PatientData.services;

public class HistoriekService : IHistoriekService
{
    
    private readonly IHistoriekRepository _historiekRepository;
    public HistoriekService(IHistoriekRepository historiekRepository)
    {
        _historiekRepository = historiekRepository;
        
    }
    public async Task<List<Message>> GetHistoriekByRange(string deviceId, DateTime start, DateTime end, GroupingRange range)
    {
        var values = await _historiekRepository.ReadRange(start, end, deviceId);
        // group by range
        var result = GroupByTime(values, range);
        return result;
    }

    private List<Message> GroupByTime(List<CosmosEntry> entries, GroupingRange range)
    {
        List<Message> result = new List<Message>();
        TimeSpan groupingInterval;

        switch (range)
        {
            case GroupingRange.Hour:
                groupingInterval = TimeSpan.FromHours(1);
                break;
            case GroupingRange.Day:
                groupingInterval = TimeSpan.FromDays(1);
                break;
            case GroupingRange.ThreeDays:
                groupingInterval = TimeSpan.FromDays(3);
                break;
            default:
                throw new ArgumentException("Invalid range specified.");
        }

        // var groupedEntries = entries.GroupBy(e => DateTimeOffset.FromUnixTimeSeconds(e.Timestamp).Date);
        
        var groupedEntries = entries.GroupBy(e =>
        {
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(e.Timestamp);
            var groupStart = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
            var groupNumber = (int)((dateTime - groupStart).TotalDays / 3);
            return groupStart.AddDays(groupNumber * 3);
        });

        
        foreach (var group in groupedEntries)
        {
            var message = new Message
            {
                Timestamp = group.Key,
                // Calculate blood pressure statistics
                Bloeddruk = CalculateBloodPressureStatistics(group)
            };

            // Calculate sensor statistics
            CalculateSensorStatistics(group.Select(e => e.AdemFrequentie), message.AdemFrequentie);
            CalculateSensorStatistics(group.Select(e => e.Hartslag), message.Hartslag);
            CalculateSensorStatistics(group.Select(e => e.Bloedzuurstof), message.Bloedzuurstof);
            CalculateSensorStatistics(group.Select(e => e.Temperatuur), message.Temperatuur);

            message.DeviceId = group.FirstOrDefault()?.DeviceId ?? "";
            result.Add(message);
        }

        return result;
    }

    private void CalculateSensorStatistics(IEnumerable<SensorValue> values, SensorHistory history)
    {
        var valueList = values.ToList(); // Convert to a list to prevent multiple enumerations
        history.Min = valueList.Min(v => v.Value);
        history.Max = valueList.Max(v => v.Value);
        history.Avg = valueList.Average(v => v.Value);
        history.Unit = valueList.FirstOrDefault()?.Unit ?? "";
    }

    private BloodPressureHistory CalculateBloodPressureStatistics(IGrouping<DateTime, CosmosEntry> grouping)
    {
        var bloodPressureValues = grouping.Select(e => e.Bloeddruk).ToList();

        var bloodPRessureHistory = new BloodPressureHistory()
        {
            SystolicMax = bloodPressureValues.Max(bp => bp.Systolic),
            SystolicMin = bloodPressureValues.Min(bp => bp.Systolic),
            SystolicAvg = (int)bloodPressureValues.Average(bp => bp.Systolic),
            DiastolicMax = bloodPressureValues.Max(bp => bp.Diastolic),
            DiastolicMin = bloodPressureValues.Min(bp => bp.Diastolic),
            DiastolicAvg = (int)bloodPressureValues.Average(bp => bp.Diastolic),
        };
        return bloodPRessureHistory;
    }
}

public interface IHistoriekService
{
    Task<List<Message>> GetHistoriekByRange(string deviceId, DateTime start, DateTime end, GroupingRange range);
}