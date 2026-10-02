using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace app.Models;


[PrimaryKey("Id")]
public class Course
{
    public Guid Id { get; init; }
    [MaxLength(50)] public string DisplayName { get; set; } = string.Empty;
    [MaxLength(300)] public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public Guid OwnerId { get; set; }
    [MaxLength(8)] public string JoinCode { get; set; } = string.Empty; 
    public required GrupperingUser Owner { get; set; }
    public List<GrupperingUser> Members { get; set; } = [];

}