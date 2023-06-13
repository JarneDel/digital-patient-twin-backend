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

var app = builder.Build();
app.UseCors();
app.MapGet("/", () => "Hello World!");

// get meldingen for one patient
app.MapGet("/meldingen/{patientId}", async (string patientId, HttpRequest request, IMeldingService meldingService) =>
{
    int start = 0;
    int end = 25;
    if (request.Query.ContainsKey("start"))
    {
        bool isParsed = int.TryParse(request.Query["start"].ToString(), out start);
        if(!isParsed) Console.WriteLine("Could not parse start");
    }
    if (request.Query.ContainsKey("end"))
    {
        bool isParsed = int.TryParse(request.Query["end"].ToString(), out end);
        if(!isParsed) Console.WriteLine("Could not parse end");
    }
    if (start > end) return Results.BadRequest("Start cannot be greater than end");
    if (end - start > 30) return Results.BadRequest("Cannot get more than 30 meldingen at once");
   
    var res = await meldingService.GetMeldingen(patientId, start, end);
    return Results.Ok(res);
});

// get meldingen for a doctor
app.MapGet("/meldingen/doctor/{dokterId}", async (string dokterId, IMeldingService meldingService) =>
{
    var res = await meldingService.GetMeldingenForDokter(dokterId);
    return Results.Ok(res);
});



app.Run();