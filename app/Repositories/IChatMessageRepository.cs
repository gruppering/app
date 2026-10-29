using app.DTO.Chat;
using app.Models;

namespace app.Repositories;

public interface IChatMessageRepository
{
    public ChatMessage GetChatMessageById(Guid messageId);
    public Task<List<ChatMessageDto>> GetAllChatMessagesByChannel(string channel);
    public Task<List<ChatMessage>> GetAllChatMessagesByUserId(Guid userId);
    public Task SaveMessage(ChatMessage message);
    public Task UpdateChatMessage(ChatMessage message);
    public Task DeleteMessage(Guid messageId);
    public Task DeleteMessage(ChatMessage message);

}