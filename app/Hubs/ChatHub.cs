using System.Security.Claims;
using app.DTO.Chat;
using app.Exceptions;
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

        try
        {
            await messageService.UpdateMessage(Guid.Parse(messageId), Guid.Parse(userId.Value), message);

            await Clients.Groups(channel).SendAsync("MessageUpdated", messageId, message);
        }
        catch (UserNotAuthorizedException)
        {
            // TODO, should add logging in the case of a user trying to change someone else's message.
            await Clients.Client(Context.ConnectionId).SendAsync("NefariousIntent","What are you trying to do?");
        }
        
    }
    

    public async Task DeleteMessage(string channel, string messageId)
    {
        var userId = Context.User!.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);

        try
        {
            await messageService.DeleteMessage(Guid.Parse(messageId), Guid.Parse(userId.Value));

            await Clients.Groups(channel).SendAsync("MessageDeleted", messageId);

        }
        catch (UserNotAuthorizedException)
        {
            // TODO, should add logging in this case as well.
            await Clients.Client(Context.ConnectionId).SendAsync("NefariousIntent","What are you trying to do?");
        }
    }
}