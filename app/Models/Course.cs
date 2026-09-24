using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace app.Models;


[PrimaryKey("Id")]
public class Course
{
    public Guid Id { get; set; }
    [MaxLength(50)]
    public string DisplayName { get; set; }
    [MaxLength(300)]
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public Guid OwnerId { get; set; }
    public GrupperingUser Owner { get; set; }
    public List<GrupperingUser> Members { get; set; }

}