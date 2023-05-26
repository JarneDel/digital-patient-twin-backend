namespace PatientData.services;

public class TimeService : ITimeService
{
    public DateTime UnixTimeStampToDateTime( double unixTimeStamp )
    {
        // Unix timestamp is seconds past epoch
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
        return dateTime;
    }

    public DateTime UnixStringToDateTime(string unixString)
    {
        return UnixTimeStampToDateTime(double.Parse(unixString));
    }
}

public interface ITimeService
{
    DateTime UnixTimeStampToDateTime( double unixTimeStamp );
    DateTime UnixStringToDateTime( string unixString );
}