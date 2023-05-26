using System.Configuration;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using PatientGegevensService.models;
using PatientGegevensService.repositories;
using PatientGegevensService.services;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

// var keyVaultName = "kvIndustryProject";
// var kvUri = "https://" + keyVaultName + ".vault.azure.net";
// var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

// var adSecret = client.GetSecretAsync("sp-patienttwin-secret").Result.Value.Value;
// 



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IPatientService, PatientService>();
builder.Services.AddSingleton<IPatientRepository, PatientRepository>();
builder.Services.AddSingleton<ISecretService, SecretService>();
builder.Services.AddDaprClient(opt => opt.UseHttpEndpoint("http://localhost:5011").UseGrpcEndpoint("http://localhost:60001"));

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    // .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));


builder.Services.AddValidatorsFromAssemblyContaining<PatientGegevens>();
var app = builder.Build();

// app.UseAuthentication();
// app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapGet("/patient/{patientId}", async (string dokterId, string patientId, IPatientService patientService) =>
{
    // todo: add authorization
    // todo: check if doctor has access to patient
    var res = await patientService.GetPatient(patientId);
    return Results.Ok(res);
});


app.MapPost("/patient/", async( PatientGegevens gegevens, IPatientService patientService, IValidator<PatientGegevens> validator, string dokterId) =>
{
    // todo: get dokterId from token
    try
    {
        var validationResult = validator.Validate(gegevens);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Results.BadRequest(errors);
        }
        // todo: add authorization
    
    
        gegevens.CreatedBy = dokterId;
        var id = await patientService.AddPatient(gegevens);
        return Results.Created($"/dokter/{dokterId}/patient/{id}", id);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.BadRequest(e.Message);
    }
    

});

app.MapPut("patient/{id}", async (PatientGegevens gegevens,string id, IPatientService service, IValidator<PatientGegevens> validator) =>
{
    try
    {
        gegevens.Id = id;
        var validationResult = validator.Validate(gegevens);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Results.BadRequest(errors);
        }
        var res = await service.UpdatePatient(gegevens);
        return Results.Ok(res);

    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.BadRequest(e.Message);
        throw;
    }


});


app.Run();