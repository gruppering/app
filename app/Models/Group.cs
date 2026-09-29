using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace app.Models;


[PrimaryKey("Id")]
public class Group
{
    public Guid Id { get; set; }
    [MaxLength(100)]
    public string GroupName { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public required Course Course { get; set; }
    public List<GrupperingUser> Members { get; set; } = [];
}