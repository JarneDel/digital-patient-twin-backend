using PatientData.models;
using PatientData.repositories;

namespace PatientData.services;

public class HistoriekService : IHistoriekService
{
    
    private readonly IHistoriekRepository _historiekRepository;
    private readonly ITimeService _timeService;
    public HistoriekService(IHistoriekRepository historiekRepository, ITimeService timeService)
    {
        _historiekRepository = historiekRepository;
        _timeService = timeService;
        
    }
    public async Task<List<Message>> GetHistoriekByRange(string deviceId, DateTime start, DateTime end, GroupingRange range)
    {
        var values = await _historiekRepository.ReadRange(start, end, deviceId);
        // group by range
        var result = GroupByTime(values, range);
        return result;
    }

    public async Task<Stats> GetStats(string deviceId, DateTime start, DateTime end)
    {
        return await _historiekRepository.ReadStats(start, end, deviceId);
    }

    private List<Message> GroupByTime(List<CosmosEntry> entries, GroupingRange range)
    {
        var result = new List<Message>();


        var groupedEntries = range switch
        {
            GroupingRange.TenMinutes => entries.GroupBy(e => DateTimeOffset.FromUnixTimeSeconds(e.Timestamp).DateTime.Minute / 10),
            GroupingRange.Day => entries.GroupBy(e => DateTimeOffset.FromUnixTimeSeconds(e.Timestamp).Date.DayOfYear),
            GroupingRange.Hour => entries.GroupBy(e => DateTimeOffset.FromUnixTimeSeconds(e.Timestamp).DateTime.Hour),
            GroupingRange.ThreeDays => entries.GroupBy(e =>
                DateTimeOffset.FromUnixTimeSeconds(e.Timestamp).Date.DayOfYear / 3),
            _ => throw new ArgumentOutOfRangeException(nameof(range), range, null)
        };

        foreach (var group in groupedEntries)
        {
            var message = new Message
            {
                Timestamp = _timeService.UnixTimeStampToDateTime(group.First().Timestamp),
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
        // history.Min = valueList.Min(v => v.Value);
        // history.Max = valueList.Max(v => v.Value);
        history.Avg = valueList.Average(v => v.Value);
        // history.Unit = valueList.FirstOrDefault()?.Unit ?? "";
    }

    private BloodPressureHistory CalculateBloodPressureStatistics<T>(IGrouping<T, CosmosEntry> grouping)
    {
        var bloodPressureValues = grouping.Select(e => e.Bloeddruk).ToList();

        var bloodPRessureHistory = new BloodPressureHistory()
        {
            // SystolicMax = bloodPressureValues.Max(bp => bp.Systolic),
            // SystolicMin = bloodPressureValues.Min(bp => bp.Systolic),
            SystolicAvg = (int)bloodPressureValues.Average(bp => bp.Systolic),
            // DiastolicMax = bloodPressureValues.Max(bp => bp.Diastolic),
            // DiastolicMin = bloodPressureValues.Min(bp => bp.Diastolic),
            DiastolicAvg = (int)bloodPressureValues.Average(bp => bp.Diastolic),
        };
        return bloodPRessureHistory;
    }
}

public interface IHistoriekService
{
    Task<List<Message>> GetHistoriekByRange(string deviceId, DateTime start, DateTime end, GroupingRange range);
    Task<Stats> GetStats(string deviceId, DateTime start, DateTime end);
    
}