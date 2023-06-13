using Microsoft.AspNetCore.SignalR;

namespace RealtimeDataService;

public class ChatHub : Hub
{
    public async Task SendMessage(string deviceId, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", deviceId, message);
    }
}