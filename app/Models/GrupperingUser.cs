using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace app.Models;

[PrimaryKey("Id")]
public class GrupperingUser
{
    public int Id { get; set; }
    public string Username { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }

}