using app.Models;

namespace app.Repositories;

public interface ICourseRepository
{
    public Task<List<Course>> GetAllPublicCoursesAsync();

    public Task<List<Course>> GetAllCoursesWithMemberAsync(GrupperingUser user);

    public Task<Course?> GetCourseByNameAsync(string courseName);
    public Task<Course> SaveAsync(Course course);
    public Task<Course> DeleteAsync(Course course);

}