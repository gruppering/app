using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace app.Models;

[PrimaryKey("Id")]
public class GrupperingUser
{
    [Column(TypeName = "uuid")]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Username { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
    
    public List<GrupperingUserRole> Roles { get; set; }

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
    
    public string Role { get; set; }
}