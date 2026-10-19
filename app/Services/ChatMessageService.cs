using app.DTO.Chat;
using app.Models;
using app.Repositories;

namespace app.Services;

public class ChatMessageService(IChatMessageRepository chatMessageRepository)
{
    
    /// <summary>
    /// Gets all chat messages from a given channel.
    /// </summary>
    /// <param name="channel">The channel id, in the case of a course, "course:FOOH" or in the case of a group "FOOH:1"</param>
    /// <returns>A list of the channel's chat messages.</returns>
    public async Task<List<ChatMessageDto>> GetAllChatMessagesInChannel(string channel)
    {
        return await chatMessageRepository.GetAllChatMessagesByChannel(channel);
    }
    
    /// <summary>
    /// Gets all chat message from a user.
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>A list of the user's chat messages.</returns>
    public async Task<List<ChatMessage>> GetAllChatMessagesByUserId(Guid userId)
    {
        return await chatMessageRepository.GetAllChatMessagesByUserId(userId);
    }
    
    /// <summary>
    /// Saves a message.
    /// </summary>
    /// <param name="message">The message to save</param>
    public async Task SaveMessage(ChatMessage message)
    {
        await chatMessageRepository.SaveMessage(message);
    }

    /// <summary>
    /// Updates a message
    /// </summary>
    /// <param name="messageId">The id of the message to update</param>
    /// <param name="userId">The id of the user sending the request</param>
    /// <param name="updatedMessage">the new message contents</param>
    public async Task UpdateMessage(Guid messageId,Guid userId, string updatedMessage)
    {
        var message = chatMessageRepository.GetChatMessageById(messageId);
        if (message.UserId.Equals(userId))
        {
            message.Message = updatedMessage;
            await chatMessageRepository.UpdateChatMessage(message);
        }
        else throw new ArgumentException("User is not the sender of the message");

        
    }

    /// <summary>
    /// Deletes a message if the supplied owner id matches the one stored in the db.
    /// </summary>
    /// <param name="messageId">The message id</param>
    /// <param name="ownerId">The id of the owner, acquired from the http context object, so someone can't send a request to delete another user's message.</param>
    public async Task DeleteMessage(Guid messageId,Guid ownerId)
    {
        if (chatMessageRepository.GetChatMessageById(messageId).UserId.Equals(ownerId))
        {
            await chatMessageRepository.DeleteMessage(messageId);
        }
        else throw new ArgumentException("User is not the sender of the message");
    }
}