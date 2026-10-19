using System.Security.Claims;
using app.DTO.Chat;
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

        var messageId = Guid.CreateVersion7();
        // Sends the message early, so the client doesn't have to wait for the db call.
        await Clients.Groups(channel).SendAsync("ReceiveMessage",new ChatMessageDto(messageId,Guid.Parse(userId.Value),userName.Value,message));
        
        var chatMessage = new ChatMessage
        {
            Id = messageId,
            Channel = channel,
            Message = message,
            UserId = Guid.Parse(userId.Value),
            Username = userName.Value
        };
        await messageService.SaveMessage(chatMessage);
    }
    
    public async Task UpdateMessage(string channel, string messageId,string message)
    {
        var userId = Context.User!.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
        
        await messageService.UpdateMessage(Guid.Parse(messageId),message);
        
        await Clients.Groups(channel).SendAsync("MessageUpdated",messageId,message);

    }
    

    public async Task DeleteMessage(string channel, string messageId)
    {
        var userId = Context.User!.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
        
        await messageService.DeleteMessage(Guid.Parse(messageId),Guid.Parse(userId.Value));
        
        await Clients.Groups(channel).SendAsync("MessageDeleted",messageId);

    }
}