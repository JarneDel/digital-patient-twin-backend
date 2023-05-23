using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using PatientData.models;
using PatientData.repositories;
using PatientData.services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDaprClient(opt => opt.UseHttpEndpoint("http://localhost:5012").UseGrpcEndpoint("http://localhost:60002"));
builder.Services.AddTransient<IHistoriekService, HistoriekService>();
builder.Services.AddTransient<ISecretService, SecretService>();
builder.Services.AddSingleton<IHistoriekRepository, HistoriekRepository>();

var app = builder.Build();

app.MapGet("/healthcheck", () => "Hello World!");

app.MapGet("/history/{patientId}", async (string patientId, IHistoriekService historiekService) =>
{
    try
    {
        // var deviceId = getDeviceId(patientId); // TODO implement
        var deviceId = "testdatagenerator";
        // var range = searchCriteria.range ?? "7d";
        // var start = searchCriteria.start ;
        // var startDateTime = DateTime.Parse(start) ?? DateTime.Now.AddDays(-7);
        // var end = searchCriteria.end;
        // var endDateTime = DateTime.Parse(end) ?? DateTime.Now;
        // var result = new List<Message>();
        // switch (range)
        // {
            // case "7d":
                // result = await historiekService.GetHistoriek7d(deviceId, startDateTime, endDateTime);
                // break;
            // default:
                // result = await historiekService.GetHistoriek7d(deviceId, startDateTime, endDateTime);
                // break;
        // }
        // return Results.Ok(result);
        var result = await historiekService.GetHistoriekByRange(deviceId, DateTime.Now.AddDays(-1), DateTime.Now, GroupingRange.Day);
        return Results.Ok(result);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.StatusCode(500);
    }
});

app.Run();