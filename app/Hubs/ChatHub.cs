using Microsoft.AspNetCore.SignalR;

namespace app.Hubs;

public class ChatHub: Hub
{
    public async Task SendMessage(Guid userId, string channel,string message)
    {
        await Clients.Groups(channel).SendAsync("ReceiveMessage",message);
    }
}