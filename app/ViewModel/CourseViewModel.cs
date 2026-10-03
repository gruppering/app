using app.Models;
using app.DTO.Course;

namespace app.ViewModel;

public class CourseViewModel(IEnumerable<Course> publicCourses, IEnumerable<Course> memberOfCourse)
{
    public IEnumerable<Course> PublicCourses => publicCourses;
    public IEnumerable<Course> MemberOfCourses => memberOfCourse;
    
    public CourseCreationDto? CreateCourse { get; set; }
}