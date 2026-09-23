using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace app.Models;

[PrimaryKey("Id")]
public class GrupperingUser
{
    [Column(TypeName = "uuid")]
    public Guid Id { get; set; }

    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(150)] [EmailAddress] public string Email { get; set; } = string.Empty;
    
    [MaxLength(75)]
    public string PasswordHash { get; set; } = string.Empty;

    public List<GrupperingUserRole> Roles { get; set; } = [];

}

[Table("UserRoles")]
[PrimaryKey("Id")]
public class GrupperingUserRole()
{
    public GrupperingUserRole(string role) : this()
    {
        Role = role;
    }
    
    public int Id { get; set; }

    public string Role { get; set; } = string.Empty;
}