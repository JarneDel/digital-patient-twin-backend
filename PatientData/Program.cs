using System.Diagnostics;
using System.Globalization;
using PatientData.models;
using PatientData.repositories;
using PatientData.services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDaprClient(opt =>
    opt.UseHttpEndpoint("http://localhost:5012").UseGrpcEndpoint("http://localhost:60002"));
builder.Services.AddTransient<IHistoriekService, HistoriekService>();
builder.Services.AddTransient<ISecretService, SecretService>();
builder.Services.AddSingleton<IHistoriekRepository, HistoriekRepository>();
builder.Services.AddSingleton<ITimeService, TimeService>();
builder.Services.AddLogging(
    loggingBuilder =>
    {
        loggingBuilder.AddFilter(level => true);
        loggingBuilder.AddConsole();
    }
);

var app = builder.Build();

app.MapGet("/healthcheck", () => "Hello World!");

app.MapGet("/history/{patientId}", async (HttpRequest req, IHistoriekService historiekService, ITimeService timeService) =>
{
    try
    {
        // var deviceId = getDeviceId(patientId); // TODO implement
        var patientId = req.RouteValues["patientId"];
        var range = req.Query["range"];
        var start = req.Query["start"].FirstOrDefault();
        var end = req.Query["end"].FirstOrDefault();
        // convert range to enum
        var groupingRange = range.FirstOrDefault()?.ToLower() switch
        {
            "1d" => GroupingRange.Hour,
            "day" => GroupingRange.Hour,
            "7d" => GroupingRange.Day,
            "week" => GroupingRange.Day,
            "30d" => GroupingRange.ThreeDays,
            "month" => GroupingRange.ThreeDays,
            _ => GroupingRange.Hour
        };
        Console.WriteLine($"Grouping range: {groupingRange}");
        // convert start and end to datetime
        var defaultStart = groupingRange switch
        {
            GroupingRange.Hour => DateTime.Now.AddDays(-1),
            GroupingRange.Day => DateTime.Now.AddDays(-7),
            GroupingRange.ThreeDays => DateTime.Now.AddDays(-30),
            _ => throw new ArgumentOutOfRangeException()
        };

        var startDateTime =  start != null ? timeService.UnixStringToDateTime(start) : defaultStart;
        var endDateTime = end != null ? timeService.UnixStringToDateTime(end) :  DateTime.Now;
        var deviceId = "testdatagenerator";
        var result =
            await historiekService.GetHistoriekByRange(deviceId, startDateTime, endDateTime, groupingRange);
        return Results.Ok(result);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.StatusCode(500);
    }
});

app.Run();