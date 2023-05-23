using System;
using Dapr.Client;
using DataGenerator.services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");

        var serviceProvider = BuildServiceProvider();
        var generator = serviceProvider.GetService<IGenerator>();
        generator?.StartGeneratingData(TimeSpan.FromSeconds(20));

        Console.WriteLine("Application started. Press Ctrl+C to stop.");

        // Wait for Ctrl+C signal (SIGINT)
        var exitEvent = new ManualResetEvent(false);
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true; // Prevent the application from shutting down immediately
            exitEvent.Set(); // Signal the exit event
        };

        // Wait for the exit event to be signaled
        exitEvent.WaitOne();

        // Stop the generator and dispose of resources
        generator?.StopGeneratingData();
        ((IDisposable)serviceProvider).Dispose();
        
        Console.WriteLine("Application stopped. Press any key to exit.");
        Console.ReadKey();
    }

    static IServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        var daprClient = new DaprClientBuilder().UseHttpEndpoint("http://localhost:5002").UseGrpcEndpoint("http://localhost:60002").Build();
        builder.AddLogging(configure => configure.AddConsole());
        builder.AddSingleton(daprClient);
        builder.AddTransient<IGenerator, Generator>();
        builder.AddTransient<IMqttService, MqttService>();
        builder.AddTransient<ISecretService, SecretService>();
        return builder.BuildServiceProvider();
    }
}