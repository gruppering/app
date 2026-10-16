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

    public Task<List<ChatMessage>> GetAllChatMessagesByUserId(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task SaveMessage(ChatMessage message)
    {
        throw new NotImplementedException();
    }

    public Task<ChatMessage> UpdateChatMessage(ChatMessage message)
    {
        throw new NotImplementedException();
    }

    public Task DeleteMessage(ChatMessage message)
    {
        throw new NotImplementedException();
    }
}