using Dokterservice.modes;
using Dokterservice.repositories;
using Dokterservice.services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDaprClient();
builder.Services.AddSingleton<ISecretService, SecretService>();
builder.Services.AddSingleton<IDokterService, DokterService>();
builder.Services.AddSingleton<IDokterRepository, DokterRepository>();

var app = builder.Build();

app.MapGet("/", () => "service is live!");

// get patients by doctor
app.MapGet("/dokter/{id}", async (string id, IDokterService dokterService) =>
{
    try
    {
        var dokter = await dokterService.GetDokter(id);
        return Results.Ok(dokter);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.NotFound();
    }
});

// get patient notification settings by patient and doctor
app.MapGet("/dokter/{id}/patient/{patientId}/notifications", async (string id, string patientId, IDokterService dokterService ) =>
{
    try
    {
        var notificationSettings = await dokterService.GetNotificationSettingsByPatient(id, patientId);
        return Results.Ok(notificationSettings);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.NotFound();
    }
});

// set patient notification settings by patient and doctor

app.MapPost("/dokter/{id}/patient/{patientId}/notifications", async (string id, string patientId, NotificationSettings notificationSettings, IDokterService dokterService) =>
{
    try
    {
        notificationSettings = await dokterService.CreateOrUpdateNotificationSettingsByPatient(id, patientId, notificationSettings);
        return Results.Ok(notificationSettings);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.BadRequest();
    }
});

// remove patient from doctor

app.MapDelete("/dokter/{id}/patient/{patientId}", async (string id, string patientId, IDokterService dokterService) =>
{
    try
    {
        var dokter = await dokterService.RemovePatientFromDokter(id, patientId);
        return Results.Ok(dokter);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.NotFound();
    }
});

// add patient to doctor
app.MapPost("/dokter/{id}/patient/{patientId}", async (string id, string patientId, IDokterService dokterService) =>
{
    try
    {
        var dokter = await dokterService.AddPatientToDokter(id, patientId);
        return Results.Ok(dokter);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.NotFound();
    }
});

// pin patient to doctor
app.MapPost("/dokter/{id}/patient/{patientId}/pin", async (string id, string patientId, IDokterService dokterService) =>
{
    try
    {
        var dokter = await dokterService.PinPatientToDokter(id, patientId);
        return Results.Ok(dokter);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.NotFound();
    }
});


// unpin patient from doctor
app.MapDelete("/dokter/{id}/patient/{patientId}/pin", async (string id, string patientId, IDokterService dokterService) =>
{
    try
    {
        var dokter = await dokterService.UnpinPatientFromDokter(id, patientId);
        return Results.Ok(dokter);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.NotFound();
    }
});




app.Run();