using app.DTO.Chat;
using app.Models;
using app.Repositories;

namespace app.Services;

public class ChatMessageService(IChatMessageRepository chatMessageRepository)
{
    
    public async Task<List<ChatMessageDto>> GetAllChatMessagesInChannel(string channel)
    {
        return await chatMessageRepository.GetAllChatMessagesByChannel(channel);
    }
    
    public async Task<List<ChatMessage>> GetAllChatMessagesByUserId(Guid userId)
    {
        return await chatMessageRepository.GetAllChatMessagesByUserId(userId);
    }
    
    
    public async Task SaveMessage(ChatMessage message)
    {
        await chatMessageRepository.SaveMessage(message);
    }

    public async Task UpdateMessage(Guid messageId, string updatedMessage)
    {
        var message = chatMessageRepository.GetChatMessageById(messageId);
        message.Message = updatedMessage;
        await chatMessageRepository.UpdateChatMessage(message);
    }

    public async Task DeleteMessage(Guid messageId,Guid ownerId)
    {
        
        await chatMessageRepository.DeleteMessage(messageId);
    }
}