using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace app.Models;


[PrimaryKey("Id")]
public class Course
{
    public Guid Id { get; set; }
    [MaxLength(50)]
    public string DisplayName { get; set; }
    public bool IsPublic { get; set; }
    public Guid GrupperingUserId { get; set; }
    public GrupperingUser Owner { get; set; }
    public List<GrupperingUser> Members { get; set; }

}