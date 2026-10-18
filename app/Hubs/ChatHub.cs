using System.Security.Claims;
using app.Models;
using app.Services;
using Microsoft.AspNetCore.SignalR;

namespace app.Hubs;

public class ChatHub(ChatMessageService messageService): Hub
{
    public async Task ConnectToChannel(string channel)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, channel);
        await Clients.Group(channel).SendAsync("UserConnected", $"{Context.User!.Claims.First( c => c.Type == ClaimTypes.NameIdentifier).Value} connected");
    }
    
    public async Task SendMessage(string channel,string message)
    {
        var userId = Context.User!.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
        var userName = Context.User!.Claims.First(c => c.Type == ClaimTypes.Name);

        
        // Sends the message early, so the client doesn't have to wait for the db call.
        await Clients.Groups(channel).SendAsync("ReceiveMessage",message);
        
        var chatMessage = new ChatMessage
        {
            Channel = channel,
            Message = message,
            UserId = Guid.Parse(userId.Value),
            Username = userName.Value
        };
        await messageService.SaveMessage(chatMessage);
    }

    public async Task DeleteMessage(string channel, string messageId)
    {
        var userId = Context.User!.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);

        // Not sure how this ideally should be done in the frontend,
        // imagining the div id becomes "message-{id}" or something, and then we remove that using JavaScript.
        await Clients.Groups(channel).SendAsync("DeletedMessage",messageId);
        
        await messageService.DeleteMessage(Guid.Parse(messageId),Guid.Parse(userId.Value));
    }
}