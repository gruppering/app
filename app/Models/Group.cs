using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace app.Models;


[PrimaryKey("Id","CourseId")]

public class Group
{
    public int Id { get; init; } 
    [MaxLength(100)]
    public string GroupName { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public required Course Course { get; set; }
    public List<GrupperingUser> Members { get; set; } = [];
}