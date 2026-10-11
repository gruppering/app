using app.Models;
using app.DTO.User;
namespace app.ViewModel;

public class CourseDetailViewModel(Course course, bool isOwner, IEnumerable<UserDetailsDto> usersInCourse,IEnumerable<Group> courseGroups)
{
    public Course Course { get; set; } = course;

    public bool IsOwner { get; set; } = isOwner;
    public IEnumerable<UserDetailsDto> UsersInCourse => usersInCourse;
    public IEnumerable<Group> GroupsInCourse => courseGroups;
}