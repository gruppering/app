using app.Models;

namespace app.Repositories;

public interface ICourseRepository
{
    public Task<List<Course>> GetAllPublicCoursesAsync();

    public Task<List<GrupperingUser>> GetAllUsersFromCourseAsync(string courseName);
    public Task<List<GrupperingUser>> GetAllUsersFromCourseByIdAsync(Guid courseId);
    
    public Task<List<Course>> GetAllCoursesWithMemberAsync(GrupperingUser user);

    public Task<Course> GetCourseByNameAsync(string courseName);
    public Task<Course> GetCourseByIdAsync(Guid id);

    public Task<Course> SaveAsync(Course course);

    public Task<Course> UpdateAsync(Course course);

    public Task<Course> DeleteAsync(Course course);

    public List<Tuple<string,Guid>> GetExistingJoinCodes();
}