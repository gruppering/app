using app.DAL;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repositories;

public class CourseRepository(GrupperingTypeContext typeContext): ICourseRepository
{
    private static readonly ILoggerFactory Factory = LoggerFactory.Create(builder => builder.AddConsole());
    private readonly ILogger _logger = Factory.CreateLogger("Courses");
    
    public async Task<Course> GetCourseByNameAsync(string courseName)
    {
        var course = await typeContext.Courses.Include(c => c.Owner).Include(c => c.Members).FirstOrDefaultAsync(c => c.DisplayName == courseName);
        if (course is not null) return course;
        _logger.LogWarning("A Course with name: {CourseName} was not found", courseName);   
        throw new ArgumentException("A course with that name was not found.");
    }
    
    public async Task<Course> GetCourseByIdAsync(Guid id)
    {
        var course = await typeContext.Courses.Include(c => c.Owner).Include(c => c.Members).FirstOrDefaultAsync(c => c.Id == id);
        if (course is not null) return course;
        
        _logger.LogWarning((string)"A Course with id: {id} was not found",id);
        throw new ArgumentException("A course with that id was not found.");
    }


    /**
     * Gets all users from the specified course.
     */
    public async Task<List<GrupperingUser>> GetAllUsersFromCourse(string courseName)
    {
        var course = await typeContext.Courses.Include(c => c.Owner).Include(c => c.Members).FirstOrDefaultAsync(c => c.DisplayName == courseName);
        if (course is not null) return course.Members;
        
        _logger.LogWarning((string)"A Course with name: {courseName} was not found",courseName);
        throw new ArgumentException("A course with that name was not found.");
        
    }
    
    /**
     * Gets all the users from the specified course id.
     */
    public async Task<List<GrupperingUser>> GetAllUsersFromCourseById(Guid courseId)
    {
        var course = await typeContext.Courses.Include(c => c.Owner).Include(c => c.Members).FirstOrDefaultAsync(c => c.Id == courseId);
        if (course is not null) return course.Members;
        _logger.LogWarning((string)"A Course with id: {courseId} was not found",courseId);
        throw new ArgumentException("A course with that name was not found.");
    }


    public async Task<List<Course>> GetAllPublicCoursesAsync()
    {
        return await typeContext.Courses.Include(c => c.Owner).Include(c => c.Members).Where(c => c.IsPublic).ToListAsync();
    }
    
    public async Task<List<Course>> GetAllCoursesWithMemberAsync(GrupperingUser member)
    {
        return await typeContext.Courses
            .Include(c => c.Owner)
            .Include(c => c.Members)
            .Where(c => c.Members.Any(g => g.Id == member.Id)).ToListAsync();
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