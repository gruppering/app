using app.Models;

namespace app.Repositories;

public interface ICourseRepository
{
    public Task<List<Course>> GetAllPublicCourses();

    public Task<List<Course>> GetAllCoursesWithMember(GrupperingUser user);

    public Task<Course?> GetCourseByName(string courseName);
    public Task<Course> Save(Course course);
}