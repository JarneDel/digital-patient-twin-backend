using meldingenservice.Models;
using meldingenservice.Repositories;
using meldingenservice.services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDaprClient();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddSingleton<ISecretService, SecretService>();
builder.Services.AddSingleton<IMeldingService, MeldingService>();
builder.Services.AddSingleton<INotificationRepository, NotificationRepository>();

var app = builder.Build();
app.UseCors();
app.MapGet("/", () => "Hello World!");

// get meldingen for one patient
app.MapGet("/meldingen/{patientId}", async (string patientId, HttpRequest request, IMeldingService meldingService) =>
{
    int offset = 0;
    Notification.NotificationType type = Notification.NotificationType.All;
    Notification.NotificationLevel level = Notification.NotificationLevel.All;
    if (request.Query.ContainsKey("offset"))
    {
        bool isParsed = int.TryParse(request.Query["offset"].ToString(), out offset);
        if (!isParsed) return Results.BadRequest($"Could not parse offset, {request.Query["offset"].ToString()}");
    }

    if (request.Query.ContainsKey("type"))
    {
        bool isParsed = Enum.TryParse(request.Query["type"].ToString(), out type);
        if (!isParsed) return Results.BadRequest($"Could not parse type, {request.Query["type"].ToString()}");
    }

    if (request.Query.ContainsKey("level"))
    {
        bool isParsed = Enum.TryParse(request.Query["level"].ToString(), out level);
        if (!isParsed) return Results.BadRequest($"Could not parse level, {request.Query["level"].ToString()}");
    }

    Console.WriteLine(
        $"Getting meldingen for patient {patientId} with offset {offset} and type {type} and level {level}");
    var res = await meldingService.GetMeldingenById(patientId, offset, level, type);
    return Results.Ok(res);
});

// get meldingen for a doctor
app.MapGet("/meldingen/dokter/{dokterId}",
    async (string dokterId, HttpRequest request, IMeldingService meldingService) =>
    {
        var offset = 0;
        var type = Notification.NotificationType.All;
        var level = Notification.NotificationLevel.All;
        if (request.Query.ContainsKey("offset"))
        {
            var isParsed = int.TryParse(request.Query["offset"].ToString(), out offset);
            if (!isParsed) return Results.BadRequest($"Could not parse offset, {request.Query["offset"].ToString()}");
        }

        if (request.Query.ContainsKey("type"))
        {
            var isParsed = Enum.TryParse(request.Query["type"].ToString(), out type);
            if (!isParsed) return Results.BadRequest($"Could not parse type, {request.Query["type"].ToString()}");
        }

        if (request.Query.ContainsKey("level"))
        {
            var isParsed = Enum.TryParse(request.Query["level"].ToString(), out level);
            if (!isParsed) return Results.BadRequest($"Could not parse level, {request.Query["level"].ToString()}");
        }

        if (request.Query.ContainsKey("patientId"))
        {
            try
            {
                var patientId = request.Query["patientId"].ToString();
                if (patientId.Length == 36) // length of guid
                {
                    Console.WriteLine("patientId: " + patientId);
                    // check if patient is in dokter's list
                    var notifications =
                        await meldingService.GetMeldingenByPatientIdAndDoctorId(patientId, dokterId, offset, level, type);
                    return Results.Ok(notifications);
                }
            }
            catch (PatientNotFoundException e)
            {
                return Results.NotFound(e.Message);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }

        var res = await meldingService.GetMeldingenByDoctorId(dokterId, offset, level, type);
        return Results.Ok(res);
    });


app.Run();