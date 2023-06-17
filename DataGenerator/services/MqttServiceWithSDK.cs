using System.Text;
using Microsoft.Azure.Devices.Client;

namespace DataGenerator.services;

public class MqttServiceWithSDK : IMqttService
{
    private readonly DeviceClient _deviceClient;
    public MqttServiceWithSDK()
    {
        var protocol = Microsoft.Azure.Devices.Client.TransportType.Mqtt;
        var connectionString = "";
        _deviceClient = DeviceClient.CreateFromConnectionString(connectionString, protocol);
    }


    public async Task PublishAsync(string topic, string payload)
    {
        await _deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(payload)));
    }

    public string GetDeviceId()
    {
        // get device twin
    }

    public async Task GetDeviceTwins()
    {
        var twin = await _deviceClient.GetTwinAsync();
        Console.WriteLine(twin.Properties.Desired["interval"]);
    }

}