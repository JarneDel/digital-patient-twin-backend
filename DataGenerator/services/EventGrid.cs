using System.Text;
using Azure;
using DataGenerator.models;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.Models;
using Newtonsoft.Json;

namespace DataGenerator.services;

public class EventGrid : ISendMessageService
{
    private EventGridPublisherClient _client;

    public EventGrid()
    {
        string topicEndpoint = "https://event-grid-iot-hub-to-frontend.northeurope-1.ts.eventgrid.azure.net/api/events";
        // string topicKey = "zwJxCgK1mmaL9V3XOAUmNHcsWVZI1dvZvPndq8CGOUw=";
        string topicKey = "WE1BDuQSmQ3ycH5jo/ZGtKh2zA3BPTRDcS25rj8HAQo=";
        Console.WriteLine("Creating EventGridPublisherClient");
        _client = new EventGridPublisherClient(new Uri(topicEndpoint), new AzureKeyCredential(topicKey));
        Console.WriteLine("Created EventGridPublisherClient");
    }

    public async Task PublishAsync(string topic, string payload)
    {
        var events = new EventGridEvent[]
        {
            new(Guid.NewGuid().ToString(), "device.telemetry", "test",
                BinaryData.FromBytes(Encoding.UTF8.GetBytes(payload)))
        };
        await _client.SendEventsAsync(events);
        
    }

    public string GetDeviceId()
    {
        throw new NotImplementedException();
    }
}

public interface ISendMessageService
{
    Task PublishAsync(string topic, string payload);
}