using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace app.Models;

[PrimaryKey("Id")]
public class ChatMessage
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public required string Channel { get; init; }
    [MaxLength(255)]
    public required string Message { get; set; }
}