using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace app.Models;

[PrimaryKey("Issuer","Subject")]
public class ExternalAuth
{
    public required string Issuer { get; set; }
    public required string Subject { get; set; }
    [Column(TypeName = "uuid")]
    public Guid UserId { get; set; }
}