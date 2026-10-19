using app.DTO.Chat;
using app.Models;
using app.DTO.User;
using Microsoft.AspNetCore.Html;

namespace app.ViewModel;

public class CourseDetailViewModel(Course course, bool isOwner, IEnumerable<UserDetailsDto> usersInCourse,IEnumerable<Group> courseGroups,string QrCodeSvg,List<ChatMessageDto> chatMessageDtos)
{
    public Course Course { get; set; } = course;

    public bool IsOwner { get; set; } = isOwner;
    public IEnumerable<UserDetailsDto> UsersInCourse => usersInCourse;
    public IEnumerable<Group> GroupsInCourse => courseGroups;
    public HtmlString QrCode = new (QrCodeSvg);
    public List<ChatMessageDto> ChatMessages = chatMessageDtos;
}