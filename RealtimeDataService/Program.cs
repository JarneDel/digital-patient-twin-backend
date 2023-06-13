
using RealtimeDataService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR()
    .AddAzureSignalR(
        "Endpoint=https://industryprojectsignalr.service.signalr.net;AccessKey=c2vBCPJySb3C1KzoF9523JYLXJQt9ZWzULmpH8igrPQ=;Version=1.0;");

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapHub<ChatHub>("/chatHub");
app.Run();