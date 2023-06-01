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

    private List<Message> GroupByTime(IEnumerable<CosmosEntry> entries, GroupingRange range)
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
            };

            // Calculate sensor statistics
            CalculateSensorStatistics(group.Select(e => e.AdemFrequentie.Value), message.AdemFrequentie);
            CalculateSensorStatistics(group.Select(e => e.Hartslag.Value), message.Hartslag);
            CalculateSensorStatistics(group.Select(e => e.Bloedzuurstof.Value), message.Bloedzuurstof);
            CalculateSensorStatistics(group.Select(e => e.Temperatuur.Value), message.Temperatuur);
            CalculateSensorStatistics(group.Select(e => e.Bloeddruk.Systolic), message.Bloeddruk.Systolic);
            CalculateSensorStatistics(group.Select(e => e.Bloeddruk.Diastolic), message.Bloeddruk.Diastolic);

            message.DeviceId = group.FirstOrDefault()?.DeviceId ?? "";
            result.Add(message);
        }

        return result;
    }

    private static void CalculateSensorStatistics(IEnumerable<decimal> values, SensorHistory history)
    {
        var valueList = values.ToList(); // Convert to a list to prevent multiple enumerations
        valueList.Sort((a, b) => a.CompareTo(b));

        var count = valueList.Count;
        var q1Index = count / 4;
        var q3Index = count - q1Index - 1;

        var q1 = valueList[q1Index];
        var q3 = valueList[q3Index];
        var iqr = q3 - q1;
    
        history.Min = Math.Round(q1 - 1.5m * iqr, 2);
        history.Max = Math.Round(q3 + 1.5m * iqr, 2 );
        history.Avg = (decimal)Math.Round(valueList.Average(v => v), 2);
        history.Q3 = q3;
        history.Q1 = q1;
    }


}

public interface IHistoriekService
{
    Task<List<Message>> GetHistoriekByRange(string deviceId, DateTime start, DateTime end, GroupingRange range);
    Task<Stats> GetStats(string deviceId, DateTime start, DateTime end);
    
}