using System.Text;
using MQTTnet;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;

namespace DataGenerator.services;

public class MqttService : IMqttService
{
    private IMqttClient? _mqttClient;
    private readonly MqttClientTcpOptions _options;

    private readonly TopicFilter[] _topicFilters =
    {
        new("devices/testdatagenerator/messages/devicebound/#", MqttQualityOfServiceLevel.AtLeastOnce),
        new("$iothub/twin/PATCH/properties/desired/#", MqttQualityOfServiceLevel.AtLeastOnce),
        new("$iothub/methods/POST/#", MqttQualityOfServiceLevel.AtLeastOnce)
    };

    public MqttService(ISecretService secretService)
    {
        var iotHubSecrets = secretService.GetIoTHubSecrets();
        _options = new MqttClientTcpOptions()
        {
            Server = iotHubSecrets.Server,
            Port = iotHubSecrets.Port,
            ClientId = iotHubSecrets.ClientId,
            UserName = iotHubSecrets.UserName,
            Password = iotHubSecrets.Password,
            ProtocolVersion = MQTTnet.Core.Serializer.MqttProtocolVersion.V311,
            TlsOptions = new MqttClientTlsOptions() { UseTls = true },
            CleanSession = true
        };
        Init().Wait();

    }
    
    private async Task Init()
    {
        var factory = new MqttClientFactory();
        _mqttClient = factory.CreateMqttClient();

        // handlers
        _mqttClient.Connected += MqttClientOnConnected;
        _mqttClient.Disconnected += MqttClientOnDisconnected;
        _mqttClient.ApplicationMessageReceived += MqttClientOnApplicationMessageReceived;

        await ConnectAndSubscribe();
    }


    private async Task ConnectAndSubscribe()
    {
        if (_mqttClient == null) return;
        await _mqttClient.ConnectAsync(_options);
        await _mqttClient.SubscribeAsync(_topicFilters);
    }

    public async Task PublishAsync(string topic, string payload)
    {
        try
        {
            if (_mqttClient == null) return;
            Console.WriteLine("publishing...");
            var message = new MqttApplicationMessage(topic, Encoding.ASCII.GetBytes(payload),
                MqttQualityOfServiceLevel.AtLeastOnce, false);
            await _mqttClient.PublishAsync(message);
            Console.WriteLine("published");
        }
        catch (Exception e)
        {
            if (e.Message.Contains("The client is not connected to the server."))
            {
                await ConnectAndSubscribe();
                await PublishAsync(topic, payload);
            }
            else
            {
                Console.WriteLine(e);
            }
        }
    }

    public string GetDeviceId()
    {
        return _options.ClientId;
    }

    private static void MqttClientOnConnected(object? sender, EventArgs e)
    {
        Console.WriteLine("Connected");
    }

    private async void MqttClientOnDisconnected(object? sender, EventArgs e)
    {
        Console.WriteLine("Disconnected");
        // delete mqttClient
        _mqttClient = null;
        // restart
        await Init();
    }

    private static void MqttClientOnApplicationMessageReceived(object? sender,
        MqttApplicationMessageReceivedEventArgs e)
    {
        Console.WriteLine(Encoding.ASCII.GetString(e.ApplicationMessage.Payload));


    }
}

public interface IMqttService
{
    Task PublishAsync(string topic, string payload);
    
    string GetDeviceId();
    
}