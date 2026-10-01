using app.DTO.Course;
using app.Models;
using app.Repositories;

namespace app.Services;

public class CourseService(ICourseRepository courseRepository,IUserRepository userRepository)
{


    
    public async Task<Course> GetCourseByNameAsync(string courseName)
    {
        return await courseRepository.GetCourseByNameAsync(courseName);
    }

    /**
     * Gets all public courses, and returns a list.
     * TODO, should probably return a public DTO or something similar, to make user info, nor internal Ids are leaked.
     */
    public async Task<List<Course>> GetAllPublicCoursesAsync()
    {
        return await courseRepository.GetAllPublicCoursesAsync();
    }

    /**
    * Gets all users from a course, and returns a list of them.
    * TODO, same case as method above, should return a list of public DTO, withholding sensitive info.
    */
    public async Task<List<GrupperingUser>> GetAllUsersFromCourse(string courseName)
    {
        return await courseRepository.GetAllUsersFromCourse(courseName);
    }

    /**
     * Gets all courses where the specified user is a member.
     */
    public async Task<List<Course>> GetAllCoursesWithMemberAsync(GrupperingUser user)
    {
        return await courseRepository.GetAllCoursesWithMemberAsync(user);
    }

    
    /**
     * Adds the specified user to the specified course. Throws an ArgumentException if a course with the given name wasn't found.
     */
    public async Task AddUserToCourse(string courseName,GrupperingUser userToAdd)
    {
        var course = await courseRepository.GetCourseByNameAsync(courseName);
        course.Members.Add(userToAdd);
        await courseRepository.UpdateAsync(course);
    }
    /**
    * Adds the user with the specified username to the specified course. Throws an ArgumentException if a course or a user with the given name wasn't found.
    */
    public async Task AddUserToCourse(string courseName,string usernameToAdd)
    {
        var course = await courseRepository.GetCourseByNameAsync(courseName);
        if (course.Members.Any(u => u.Username == usernameToAdd)) throw new ArgumentException("A user with that name already is a member of this course.");
        var user = userRepository.GetUserByUsername(usernameToAdd);
        if (user is null) throw new ArgumentException("A user with that name was not found"); // TODO, find better way of handling this
        course.Members.Add(user);
        await courseRepository.UpdateAsync(course);
    }

    /**
     * Removes the specified user from the specified course. Throws an ArgumentException if a course with the given name wasn't found or the user isn't a member of the specified course.
     */
    public async Task RemoveUserFromCourse(string courseName,GrupperingUser userToRemove)
    {
        var course = await courseRepository.GetCourseByNameAsync(courseName);
        var user = course.Members.Find(u => u.Id == userToRemove.Id);
        if (user is null) throw new ArgumentException("User was not found as a member of this course.");
        course.Members.Remove(user);
        await courseRepository.UpdateAsync(course);
    }
    /**
     * Removes the specified user from the specified course. Throws an ArgumentException if a course or user with the given name wasn't found.
     */
    public async Task RemoveUserFromCourse(string courseName,string usernameToRemove)
    {
        var course = await courseRepository.GetCourseByNameAsync(courseName);
        var user = course.Members.Find(u => u.Username == usernameToRemove);
        if (user is null) throw new ArgumentException("User was not found as a member of this course.");
        course.Members.Remove(user);
        await courseRepository.UpdateAsync(course);
    }
    
    /**
     * Creates a new course given the creationDto and a creator, envisioned to be the user making the request.
     */
    public async Task<Course> CreateNewCourse(CourseCreationDto creationDto,GrupperingUser creator)
    {
        var course = new Course
        {
            DisplayName = creationDto.CourseName,
            Description = creationDto.Description,
            Owner = creator,
            OwnerId = creator.Id,
            IsPublic = creationDto.IsPublic
        };
        return await courseRepository.SaveAsync(course);
    }
    
    /**
     * Deletes a course with the given name, given that the user is authorized to do so, throws an ArgumentException if the specified course wasn't found.
     * If the user requesting the deletion isn't authorized to do that, an ArgumentException is thrown.
     */
    public async Task<Course> DeleteCourse(string courseName,GrupperingUser requestor)
    {
        if (requestor.Roles.Any(r => r.Role == "ROLE_ADMIN"))
            throw new ArgumentException("User is not authorized to complete this action.");
        var course = await courseRepository.GetCourseByNameAsync(courseName);
        return await courseRepository.DeleteAsync(course);
    }
}