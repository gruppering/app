using app.DTO.Chat;
using app.Models;
using app.DTO.User;
using Microsoft.AspNetCore.Html;

namespace app.ViewModel;

public class GroupDetailViewModel(Group group, Course course, bool isOwner)
{
    public Group Group { get; set; } = group;
    public Course Course { get; set; } = course;
    public bool IsOwner { get; set; } = isOwner;
}