// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography.X509Certificates;
using System.Text;
using Dapr.Client;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// var daprClient = new DaprClientBuilder().Build();
// var ioTHubConnectionString = await daprClient.GetSecretAsync("azure-iot-hub", "iothub-connection-string");
const string hostName = "mqtttestdelaware.azure-devices.net/";
const string userName = "mqtttestdelaware.azure-devices.net/testdatagenerator?api-version=2021-04-12";
const string passw =
    "SharedAccessSignature sr=mqtttestdelaware.azure-devices.net%2Fdevices%2Ftestdatagenerator&sig=M1qH3JO88KG7MSXHh3DZBidkjKl1W2XWoDjY0QF4Ta0%3D&se=1687755434";
const string clientId = "testdatagenerator";
const bool cleanSession = true;


var mqttFactory = new MqttFactory();
var mqttClient = mqttFactory.CreateMqttClient();
var mqttClientOptions = new MqttClientOptionsBuilder()
    .WithTcpServer(hostName, 8883)
    .WithCredentials(userName, passw)
    .WithClientId(clientId)
    .WithCleanSession(cleanSession)
    .WithTls(new MqttClientOptionsBuilderTlsParameters()
    {
        AllowUntrustedCertificates = true,
        Certificates = new List<X509Certificate>
        {
            new X509Certificate2("DigiCert_Global_Root_G2.pem"),
        },
        UseTls = true,
    })
    .Build();

Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
{
    Console.WriteLine("Message received: " + Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
    return Task.CompletedTask;
}


mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

var payloadJson = new JObject();
payloadJson.Add("temp", DateTime.UtcNow.Millisecond % 20);
payloadJson.Add("hum", DateTime.UtcNow.Millisecond / 10);
string payloadString = JsonConvert.SerializeObject(payloadJson);
var message = new MqttApplicationMessageBuilder()
    .WithTopic("devices/testdatagenerator/messages/events/")
    .WithPayload(payloadString)
    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
    .Build();
 
await mqttClient.PublishAsync(message);