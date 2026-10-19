using System.Diagnostics;
using app.DAL;
using app.DTO.Chat;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repositories;

public class ChatMessageRepository(GrupperingTypeContext typeContext): IChatMessageRepository
{
    
    /// <summary>
    /// Gets a chat message by its id.
    /// </summary>
    /// <param name="messageId">The id of the message</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Throws when a message with this id is not found.</exception>
    public ChatMessage GetChatMessageById(Guid messageId)
    {
        var message =  typeContext.ChatMessages.Find(messageId);
        return message ?? throw new ArgumentException("Message with this id does not exist");
    }

    /// <summary>
    /// Gets all messages from a specific channel, ordered by the id, as the timestamp is the first 12 characters of the id.
    /// Uses AsNoTracking as we won't update these elements using this method, so we can enjoy a somewhat faster query.
    /// </summary>
    /// <param name="channel">The channel name, either in the form of "course:FOOH", or "FOOH:1" for a group</param>
    /// <returns>A list of all chat messages for this channel.</returns>
    public async Task<List<ChatMessageDto>> GetAllChatMessagesByChannel(string channel)
    {
        return await typeContext.ChatMessages.Where(cm => cm.Channel == channel).OrderBy(cm => cm.Id).AsNoTracking().Select(c => new ChatMessageDto(c.Id,c.UserId,c.Username,c.Message)).ToListAsync();
    }

    /// <summary>
    /// Gets all chat messages by a specific user.
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>A list of all chat messages by a given user.</returns>
    public async Task<List<ChatMessage>> GetAllChatMessagesByUserId(Guid userId)
    {
        return await typeContext.ChatMessages.Where(cm => cm.UserId == userId).ToListAsync();
    }

    
    /// <summary>
    /// Saves a message
    /// </summary>
    /// <param name="message">The message to save</param>
    public async Task SaveMessage(ChatMessage message)
    {
        await typeContext.ChatMessages.AddAsync(message);
        await typeContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates a message.
    /// </summary>
    /// <param name="message">The message to update.</param>
    public async Task UpdateChatMessage(ChatMessage message)
    {
        typeContext.ChatMessages.Update(message);
        await typeContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// Deletes the message with the given id, or throws an argument exception.
    /// </summary>
    /// <param name="messageId">The id of the message.</param>
    public async Task DeleteMessage(Guid messageId)
    {
        var message = await typeContext.ChatMessages.FindAsync(messageId);
        if (message is null) throw new ArgumentException("This message does not exist.");
        typeContext.ChatMessages.Remove(message);
        await typeContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// Deletes a chat message.
    /// </summary>
    /// <param name="message">The message to be deleted.</param>
    public async Task DeleteMessage(ChatMessage message)
    {
        typeContext.ChatMessages.Remove(message);
        await typeContext.SaveChangesAsync();
    }
}