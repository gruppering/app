using app.Models;

namespace app.ViewModel;

public class CourseDetailViewModel(Course course, bool isOwner, IEnumerable<GrupperingUser> usersInCourse)
{
    public Course Course { get; set; } = course;

    public bool IsOwner { get; set; } = isOwner;
    public IEnumerable<GrupperingUser> UsersInCourse => usersInCourse;
}