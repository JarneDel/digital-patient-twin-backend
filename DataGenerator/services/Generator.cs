using DataGenerator.models;
using MQTTnet.Core.Client;

namespace DataGenerator.services;

public class Generator : IGenerator
{

    private readonly string _topic = "devices/testdatagenerator/messages/events/$.ct=application%2Fjson&$.ce=utf-8";
    private readonly IMqttService _mqttService;
    private Timer? _timer;
    
    public Generator(IMqttService mqttService)
    {
        _mqttService = mqttService;
    }

    public void StartGeneratingData(TimeSpan interval)
    {
        if (_timer != null)
        {
            throw new InvalidOperationException("Data generation is already in progress.");
        }

        _timer = new Timer(GenerateAndSendData, null, TimeSpan.Zero, interval);
    }

    public void StopGeneratingData()
    {
        _timer?.Dispose();
        _timer = null;
    }
    
    private void GenerateAndSendData(object? state)
    {
        var random = new Random();

        var data = new Message
        {
            Bloeddruk = new BloodPressure
            {
                Systolic = random.Next(80, 180),
                Diastolic = random.Next(50, 110),
                Unit = "mmHg"
            },
            AdemFrequentie = new SensorValue
            {
                Value = random.Next(10, 20),
                Unit = "/min"
            },
            Hartslag = new SensorValue
            {
                Value = random.Next(60, 140),
                Unit = "bpm"
            },
            Bloedzuurstof = new SensorValue
            {
                Value = random.Next(90, 100),
                Unit = "%"
            },
            Temperatuur = new SensorValue
            {
                Value = (decimal)(random.NextDouble() * (37.5 - 36.0) + 36.0),
                Unit = "°C"
            },
            DeviceId = _mqttService.GetDeviceId()
            // DeviceId = "testdatagenerator"
        };

        
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        try
        {
            _mqttService.PublishAsync(_topic, json).Wait();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}


public interface IGenerator
{
    void StartGeneratingData(TimeSpan interval);
    void StopGeneratingData();
    
}