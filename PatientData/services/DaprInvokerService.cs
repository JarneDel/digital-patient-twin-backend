using Dapr.Client;
using PatientData.models;

namespace PatientData.services;

public class DaprInvokerService : IDaprInvokerService
{
    private readonly DaprClient _daprClient;
    public DaprInvokerService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<PatientGegevens> GetPatient(string id)
    {
        var requestMesssage = _daprClient.CreateInvokeMethodRequest<string>(HttpMethod.Get, "patientgegevensservice", "patient/" + id, null);
        Console.WriteLine("Getting Patient with request: " + requestMesssage.RequestUri?.AbsoluteUri);
        var response = await _daprClient.InvokeMethodAsync<PatientGegevens>(requestMesssage);
        Console.WriteLine("Got response: " + response.Algemeen.Naam);
        return response;
    }
}

public interface IDaprInvokerService
{
    Task<PatientGegevens> GetPatient(string id);
}