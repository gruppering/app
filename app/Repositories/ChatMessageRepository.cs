using app.DAL;
using app.DTO.Chat;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repositories;

public class ChatMessageRepository(GrupperingTypeContext typeContext): IChatMessageRepository
{
    
    public ChatMessage GetChatMessageById(Guid messageId)
    {
        var message =  typeContext.ChatMessages.Find(messageId);
        return message ?? throw new ArgumentException("Message with this id does not exist");
    }

    public async Task<List<ChatMessageDto>> GetAllChatMessagesByChannel(string channel)
    {
        return await typeContext.ChatMessages.Where(cm => cm.Channel == channel).Select(c => new ChatMessageDto(c.UserId,c.Username,c.Message)).ToListAsync();
    }

    public async Task<List<ChatMessage>> GetAllChatMessagesByUserId(Guid userId)
    {
        return await typeContext.ChatMessages.Where(cm => cm.UserId == userId).ToListAsync();
    }

    public async Task SaveMessage(ChatMessage message)
    {
        await typeContext.ChatMessages.AddAsync(message);
        await typeContext.SaveChangesAsync();
    }

    public async Task UpdateChatMessage(ChatMessage message)
    {
        typeContext.ChatMessages.Update(message);
        await typeContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="messageId"></param>
    public async Task DeleteMessage(Guid messageId)
    {
        var message = await typeContext.ChatMessages.FindAsync(messageId);
        if (message is null) throw new ArgumentException("This message does not exist.");
        typeContext.ChatMessages.Remove(message);
        await typeContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public async Task DeleteMessage(ChatMessage message)
    {
        typeContext.ChatMessages.Remove(message);
        await typeContext.SaveChangesAsync();
    }
}