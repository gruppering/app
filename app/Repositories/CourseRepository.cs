using app.DAL;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repositories;

public class CourseRepository(GrupperingTypeContext typeContext): ICourseRepository
{
    
    public async Task<Course> GetCourseByNameAsync(string courseName)
    {
        var course = await typeContext.Courses.Include(c => c.Owner).Include(c => c.Members).FirstOrDefaultAsync(c => c.DisplayName == courseName);
        return course ?? throw new ArgumentException("A course with that name was not found.");
    }
    
    public async Task<Course> GetCourseByIdAsync(Guid id)
    {
        var course = await typeContext.Courses.Include(c => c.Members).FirstAsync(c => c.Id == id);
        return course ?? throw new ArgumentException("A course with that name was not found.");
    }


    /**
     * Gets all users from the specified course.
     */
    public async Task<List<GrupperingUser>> GetAllUsersFromCourse(string courseName)
    {
        var course = await typeContext.Courses.Include(c => c.Members).FirstOrDefaultAsync(c => c.DisplayName == courseName);
        if (course is null) throw new ArgumentException("A course with that name was not found.");
        return course.Members;
    }
    
    /**
     * Gets all the users from the specified course id.
     */
    public async Task<List<GrupperingUser>> GetAllUsersFromCourseById(Guid courseId)
    {
        var course = await typeContext.Courses.Include(c => c.Members).FirstOrDefaultAsync(c => c.Id == courseId);
        if (course is null) throw new ArgumentException("A course with that name was not found.");
        return course.Members;
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
    
    public async Task<Course> UpdateAsync(Course course)
    {
        var courseEntity = typeContext.Courses.Update(course);
        await typeContext.SaveChangesAsync();
        return courseEntity.Entity;
    }
    public async Task<Course> DeleteAsync(Course course)
    {
        var courseEntity = typeContext.Courses.Remove(course);
        await typeContext.SaveChangesAsync();
        return courseEntity.Entity;
    }

    public List<Tuple<string,Guid>> GetExistingJoinCodes()
    {
        return typeContext.Courses.Select(c => new Tuple<string,Guid>(c.JoinCode,c.Id)).ToList();
    }
}