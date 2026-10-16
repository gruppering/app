using app.Models;

namespace app.Repositories;

public interface IChatMessageRepository
{
    public Task<List<ChatMessage>> GetAllChatMessagesByChannel(string channel);
    public Task<List<ChatMessage>> GetAllChatMessagesByUserId(Guid userId);
    public Task SaveMessage(ChatMessage message);
    public Task UpdateChatMessage(ChatMessage message);
    public Task DeleteMessage(ChatMessage message);
}