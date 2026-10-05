using app.Models;

namespace app.ViewModel;

public class CourseDetailViewModel(Course course, bool isOwner, IEnumerable<GrupperingUser> usersInCourse,IEnumerable<Group> courseGroups)
{
    public Course Course { get; set; } = course;

    public bool IsOwner { get; set; } = isOwner;
    public IEnumerable<GrupperingUser> UsersInCourse => usersInCourse;
    public IEnumerable<Group> GroupsInCourse => courseGroups;
}