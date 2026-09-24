using app.DAL;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repositories;

public class CourseRepository(GrupperingTypeContext typeContext): ICourseRepository
{
    
    public async Task<Course?> GetCourseByNameAsync(string courseName)
    {
        return await typeContext.Courses.Include(c => c.Members).FirstOrDefaultAsync(c => c.DisplayName == courseName);
    }

    public async Task<List<Course>> GetAllPublicCoursesAsync()
    {
        return await typeContext.Courses.Include(c => c.Members).Where(c => c.IsPublic).ToListAsync();
    }
    
    public async Task<List<Course>> GetAllCoursesWithMemberAsync(GrupperingUser member)
    {
        return await typeContext.Courses.Include(c => c.Members).Where(c => c.Members.Contains(member)).ToListAsync();
    }
    
    public async Task<Course> SaveAsync(Course course)
    {
        var courseEntity = typeContext.Courses.Add(course);
        await typeContext.SaveChangesAsync();
        return courseEntity.Entity;
    }
}