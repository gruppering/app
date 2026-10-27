using app.DTO.Course;
using app.DTO.User;
using app.Exceptions;
using app.Models;
using app.Repositories;
using app.Utils;

namespace app.Services;

public class CourseService
{
    // We could manually search by join code in the db,
    // but this is much, much quicker, and saves us an extra trip to the db. 
    private readonly Dictionary<string,Guid> _joinCodeOverview = new ();
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public CourseService(ICourseRepository courseRepository,IUserRepository userRepository)
    {
        _courseRepository = courseRepository;
        _userRepository = userRepository;
        GetExistingJoinCodesAndPopulate();
    }

    // Internal method to populate join code overview.
    private void GetExistingJoinCodesAndPopulate()
    {
        List<Tuple<string,Guid>> joinCodes = _courseRepository.GetExistingJoinCodes();
        foreach (var j in joinCodes)
        {
            _joinCodeOverview.Add(j.Item1,j.Item2);
        }
    }
    
    /// <summary>
    /// Gets a course by the specified join code
    /// </summary>
    /// <param name="joinCode">The join code of a course</param>
    /// <returns>A course</returns>
    /// <exception cref="CourseNotFoundException">Throws if a course with the specified join code was not found</exception>
    public async Task<Course> GetCourseByJoinCodeAsync(string joinCode)
    {
        if (_joinCodeOverview.TryGetValue(joinCode, out Guid id))
            return await _courseRepository.GetCourseByIdAsync(id);
        throw new CourseNotFoundException("A course with this join code was not found.");
    }

    // Self-explanatory.
    public async Task<Course> GetCourseByNameAsync(string courseName)
    {
        return await _courseRepository.GetCourseByNameAsync(courseName);
    }

    /// <summary>
    /// Gets all the public courses
    /// </summary>
    /// <returns>A list of all public courses</returns>
    public async Task<List<Course>> GetAllPublicCoursesAsync()
    {
        return await _courseRepository.GetAllPublicCoursesAsync();
    }

    /// <summary>
    /// Gets all the users from the specified course
    /// </summary>
    /// <param name="courseName">The name of the course</param>
    /// <returns>A list of user detail dtos</returns>
    public async Task<List<UserDetailsDto>> GetAllUsersFromCourseAsync(string courseName)
    {
        var users = await _courseRepository.GetAllUsersFromCourseAsync(courseName);
        var dto = users.Select(item => new UserDetailsDto(item.Username));
        
        return dto.ToList();
    }
    
    /// <summary>
    /// Same as the method above, but uses the id so we can take advantage of the db index.
    /// </summary>
    /// <param name="courseId">The id of the course</param>
    /// <returns>a list of the users in a course.</returns>
    public async Task<List<UserDetailsDto>> GetAllUsersFromCourseByIdAsync(Guid courseId)
    {
        var users = await _courseRepository.GetAllUsersFromCourseByIdAsync(courseId);
        var dto = users.Select(item => new UserDetailsDto(item.Username));
        
        return dto.ToList();
    }

    /// <summary>
    /// Gets all courses the user is a member in
    /// </summary>
    /// <param name="user">The user to look for</param>
    /// <returns>A list of all the courses the user is in</returns>
    public async Task<List<Course>> GetAllCoursesWithMemberAsync(GrupperingUser user)
    {
        return await _courseRepository.GetAllCoursesWithMemberAsync(user);
    }

    
    /// <summary>
    /// Adds a user to the specified course
    /// </summary>
    /// <param name="courseName">The name of the course</param>
    /// <param name="userToAdd">The user to add</param>
    public async Task AddUserToCourseAsync(string courseName,GrupperingUser userToAdd)
    {
        var course = await _courseRepository.GetCourseByNameAsync(courseName);
        course.Members.Add(userToAdd);
        await _courseRepository.UpdateAsync(course);
    }
    
    /// <summary>
    /// Adds the specified user to a course
    /// </summary>
    /// <param name="courseId">The id of the course </param>
    /// <param name="userId">The id of the user to add</param>
    /// <exception cref="UserAlreadyExistsException">Throws if a user with that id was not found or is already in the course.</exception>
    public async Task AddUserToCourseAsync(Guid courseId,string userId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course.Members.Any(u => u.Id.ToString() == userId)) throw new UserAlreadyExistsException("A user with that name already is a member of this course.");
        
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null) throw new UserNotFoundException();
        
        course.Members.Add(user);
        await _courseRepository.UpdateAsync(course);
    }
    
    /// <summary>
    /// Adds a list of users to a course
    /// </summary>
    /// <param name="courseId">The course id to add users to</param>
    /// <param name="usersToAdd">The list of users to add to a course</param>
    public async Task AddUsersToCourseAsync(Guid courseId,List<GrupperingUser> usersToAdd)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        course.Members.AddRange(usersToAdd);
        await _courseRepository.UpdateAsync(course);
    }

    /// <summary>
    /// Removes the given user from the course, by the name of the course and the user.
    /// </summary>
    /// <param name="courseName">The name of the course to remove the user from</param>
    /// <param name="userToRemove">The user to remove</param>
    /// <exception cref="CourseNotFoundException">Throws if the course wasn't found</exception>
    /// <exception cref="UserNotFoundException">Throws if the  user wasn't found</exception>
    public async Task RemoveUserFromCourseAsync(string courseName,GrupperingUser userToRemove)
    {
        var course = await _courseRepository.GetCourseByNameAsync(courseName);
        var user = course.Members.Find(u => u.Id == userToRemove.Id);
        if (user is null) throw new UserNotFoundException("User was not found as a member of this course.");
        course.Members.Remove(user);
        await _courseRepository.UpdateAsync(course);
    }
    
    /// <summary>
    /// Removes the given user from the course, by the name of the course and the username of the user.
    /// </summary>
    /// <param name="courseName">The name of the course to remove the user from</param>
    /// <param name="usernameToRemove">The username of the user to remove</param>
    /// <exception cref="CourseNotFoundException">Throws if the course wasn't found</exception>
    /// <exception cref="UserNotFoundException">Throws if the  user wasn't found</exception>
    public async Task RemoveUserFromCourseAsync(string courseName,string usernameToRemove)
    {
        var course = await _courseRepository.GetCourseByNameAsync(courseName);
        var user = course.Members.Find(u => u.Username == usernameToRemove);
        if (user is null) throw new UserNotFoundException("User was not found as a member of this course.");
        course.Members.Remove(user);
        await _courseRepository.UpdateAsync(course);
    }
    
    /// <summary>
    /// Creates a new course.
    /// </summary>
    /// <param name="creationDto">The Dto from the request.</param>
    /// <param name="creator">The user creating the course, acquired from the request as well</param>
    /// <returns>The newly created course</returns>
    public async Task<Course> CreateNewCourseAsync(CourseCreationDto creationDto,GrupperingUser creator)
    {
        var guid = Guid.CreateVersion7();
        var joinCode = JoinCodeUtils.GenerateNonRandomJoinCode(_joinCodeOverview.Keys, 4);
        var course = new Course
        {
            Id = guid,
            DisplayName = creationDto.CourseName,
            Description = creationDto.Description,
            Owner = creator,
            OwnerId = creator.Id,
            IsPublic = creationDto.IsPublic,
            JoinCode = joinCode
        };
        var savedCourse = await _courseRepository.SaveAsync(course);
        _joinCodeOverview.Add(joinCode,savedCourse.Id);
        return savedCourse;
    }

    /// <summary>
    /// Deletes a course with the given name, given that the user is authorized to do so.
    /// If the user requesting the deletion isn't authorized to do that, an ArgumentException is thrown.
    /// </summary>
    /// <param name="courseName">The course to deleted</param>
    /// <param name="requestor">The user requesting the deletion</param>
    /// <returns>The deleted course</returns>
    /// <exception cref="UserNotAuthorizedException">Throws if the user isn't authorized to delete this course</exception>
    /// <exception cref="CourseNotFoundException">Throws if the specified course wasn't found</exception>
    public async Task<Course> DeleteCourseAsync(string courseName,GrupperingUser requestor)
    {
        if (requestor.Roles.Any(r => r.Role == "ROLE_ADMIN"))
            throw new UserNotAuthorizedException("User is not authorized to complete this action.");
        var course = await _courseRepository.GetCourseByNameAsync(courseName);
        _joinCodeOverview.Remove(course.JoinCode);
        return await _courseRepository.DeleteAsync(course);
    }
}