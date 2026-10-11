using app.DTO.Course;
using app.DTO.User;
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
    
    /**
     * Gets a course by the specified join code by using the lookup dictionary we populate on service creation.
     */
    public async Task<Course> GetCourseByJoinCodeAsync(string joinCode)
    {
        if (_joinCodeOverview.TryGetValue(joinCode, out Guid id))
            return await _courseRepository.GetCourseByIdAsync(id);
        throw new ArgumentException("A course with this join code was not found.");
    }

    // Self-explanatory.
    public async Task<Course> GetCourseByNameAsync(string courseName)
    {
        return await _courseRepository.GetCourseByNameAsync(courseName);
    }

    /**
     * Gets all public courses, and returns a list.
     * TODO, should probably return a public DTO or something similar, to make user info, nor internal Ids are leaked.
     */
    public async Task<List<Course>> GetAllPublicCoursesAsync()
    {
        return await _courseRepository.GetAllPublicCoursesAsync();
    }

    /**
    * Gets all users from a course, and returns a list of them.
    * TODO, same case as method above, should return a list of public DTO, withholding sensitive info.
    */
    public async Task<List<UserDetailsDto>> GetAllUsersFromCourseAsync(string courseName)
    {
        var users = await _courseRepository.GetAllUsersFromCourse(courseName);
        var dto = users.Select(item => new UserDetailsDto(item.Username));
        
        return dto.ToList();
    }

    /**
     * Gets all courses where the specified user is a member.
     */
    public async Task<List<Course>> GetAllCoursesWithMemberAsync(GrupperingUser user)
    {
        return await _courseRepository.GetAllCoursesWithMemberAsync(user);
    }

    
    /**
     * Adds the specified user to the specified course. Throws an ArgumentException if a course with the given name wasn't found.
     */
    public async Task AddUserToCourseAsync(string courseName,GrupperingUser userToAdd)
    {
        var course = await _courseRepository.GetCourseByNameAsync(courseName);
        course.Members.Add(userToAdd);
        await _courseRepository.UpdateAsync(course);
    }
    /**
    * Adds the user with the specified username to the specified course. Throws an ArgumentException if a course or a user with the given name wasn't found.
    */
    public async Task AddUserToCourseAsync(string courseName,string usernameToAdd)
    {
        var course = await _courseRepository.GetCourseByNameAsync(courseName);
        if (course.Members.Any(u => u.Username == usernameToAdd)) throw new ArgumentException("A user with that name already is a member of this course.");
        var user = _userRepository.GetUserByUsername(usernameToAdd);
        if (user is null) throw new ArgumentException("A user with that name was not found"); // TODO, find better way of handling this
        course.Members.Add(user);
        await _courseRepository.UpdateAsync(course);
    }
    /**
     * Adds the specified users to the specified course. Throws an ArgumentException if a course with the given name wasn't found.
     */
    public async Task AddUsersToCourseAsync(Guid courseId,List<GrupperingUser> usersToAdd)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        course.Members.AddRange(usersToAdd);
        await _courseRepository.UpdateAsync(course);
    }

    /**
     * Removes the specified user from the specified course. Throws an ArgumentException if a course with the given name wasn't found or the user isn't a member of the specified course.
     */
    public async Task RemoveUserFromCourseAsync(string courseName,GrupperingUser userToRemove)
    {
        var course = await _courseRepository.GetCourseByNameAsync(courseName);
        var user = course.Members.Find(u => u.Id == userToRemove.Id);
        if (user is null) throw new ArgumentException("User was not found as a member of this course.");
        course.Members.Remove(user);
        await _courseRepository.UpdateAsync(course);
    }
    /**
     * Removes the specified user from the specified course. Throws an ArgumentException if a course or user with the given name wasn't found.
     */
    public async Task RemoveUserFromCourseAsync(string courseName,string usernameToRemove)
    {
        var course = await _courseRepository.GetCourseByNameAsync(courseName);
        var user = course.Members.Find(u => u.Username == usernameToRemove);
        if (user is null) throw new ArgumentException("User was not found as a member of this course.");
        course.Members.Remove(user);
        await _courseRepository.UpdateAsync(course);
    }
    
    /**
     * Creates a new course given the creationDto and a creator, envisioned to be the user making the request.
     */
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
    
    /**
     * Deletes a course with the given name, given that the user is authorized to do so, throws an ArgumentException if the specified course wasn't found.
     * If the user requesting the deletion isn't authorized to do that, an ArgumentException is thrown.
     */
    public async Task<Course> DeleteCourseAsync(string courseName,GrupperingUser requestor)
    {
        if (requestor.Roles.Any(r => r.Role == "ROLE_ADMIN"))
            throw new ArgumentException("User is not authorized to complete this action.");
        var course = await _courseRepository.GetCourseByNameAsync(courseName);
        _joinCodeOverview.Remove(course.JoinCode);
        return await _courseRepository.DeleteAsync(course);
    }
}