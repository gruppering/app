using app.DAL;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repositories;

public class ChatMessageRepository(GrupperingTypeContext typeContext): IChatMessageRepository
{
    public async Task<List<ChatMessage>> GetAllChatMessagesByChannel(string channel)
    {
        return await typeContext.ChatMessages.Where(cm => cm.Channel == channel).ToListAsync();
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
}