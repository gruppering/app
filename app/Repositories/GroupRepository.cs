using app.DAL;
using app.Exceptions;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repositories;

public class GroupRepository(GrupperingTypeContext typeContext): IGroupRepository
{
    
    /// <summary>
    /// Gets all the groups of a user by their username
    /// </summary>
    /// <param name="userName">the username of the user</param>
    /// <returns>a list of the groups the user is a member of</returns>
    public async Task<List<Group>> GetUserGroups(string userName)
    {
        return await typeContext.Groups.Include(g => g.Members)
            .Where(g => g.Members.Any(gu => gu.Username == userName ))
            .ToListAsync();
    }

    /// <summary>
    /// Gets a group by course id and group name
    /// </summary>
    /// <param name="name">The group name</param>
    /// <param name="courseId">The course id</param>
    /// <returns>A group</returns>
    /// <exception cref="GroupNotFoundException">Thrown if a group with this name was not found in the specified course</exception>
    public Group GetGroupByNameAndCourseId(string name, Guid courseId)
    {
        try
        {
            return typeContext.Groups.Include(g => g.Members).First(g => g.GroupName == name && g.CourseId == courseId);
        }
        catch (ArgumentNullException)
        {
            throw new GroupNotFoundException("A group with this name in this course was not found");
        }
    }
    
    /// <summary>
    /// Gets all groups in the specified course by the specified prefix
    /// </summary>
    /// <param name="namePrefix">The prefix to search for</param>
    /// <param name="courseId">The course id</param>
    /// <returns>A list of groups with the given name prefix</returns>
    /// <exception cref="GroupNotFoundException">Thrown if no groups with this prefix was found</exception>
    public async Task<List<Group>> GetGroupsByNamePrefixAndCourseId(string namePrefix, Guid courseId)
    {
        try
        {
            return await typeContext.Groups.Where(g => g.GroupName.StartsWith(namePrefix) && g.CourseId == courseId)
                .ToListAsync();
        }
        catch (ArgumentNullException)
        {
            throw new GroupNotFoundException("Groups with this name prefix was not found");
        }
    }

    /// <summary>
    /// Gets a group by the course id and group id.
    /// </summary>
    /// <param name="groupId">the id of the group</param>
    /// <param name="courseId">the course id</param>
    /// <returns>a group</returns>
    /// <exception cref="GroupNotFoundException">Thrown if a group with this id was not found</exception>
    public async Task<Group> GetGroupByCourseIdAndId(int groupId, Guid courseId)
    {
        try
        {
            return await typeContext.Groups.FirstAsync(g => g.Id == groupId && g.CourseId == courseId);
        }
        catch (ArgumentNullException)
        {
            throw new GroupNotFoundException("A group in this course with this id was not found");
        }
    }

    /// <summary>
    /// Gets all groups from the specified course.
    /// </summary>
    /// <param name="courseId">The course id</param>
    /// <returns>A list of groups</returns>
    /// <exception cref="CourseNotFoundException">Thrown if the course doesn't exist or if it has no groups</exception>
    public async Task<List<Group>> GetAllGroupsByCourse(Guid courseId)
    {
        try
        {
            return await typeContext.Groups.Include(g => g.Members).Where(g => g.CourseId == courseId).ToListAsync();
        }
        catch (ArgumentNullException)
        {
            throw new CourseNotFoundException("This course does not exist, or this course has no groups.");
        }
    }

    /// <summary>
    /// Creates a new group in the specified course with the specified name, as names aren't required to be unique.
    /// </summary>
    /// <param name="name">The name of the new group</param>
    /// <param name="course">The course to add the new group to</param>
    /// <returns>The newly created group</returns>
    public async Task<Group> CreateNewGroupAsync(string name, Course course)
    {
        // If there already exists groups connected to the given course, it returns the largest id.
        // If not, it returns 0, allowing us to start from 1 as the id as seen below when creating the group.
        var nextId =  await typeContext.Groups
            .Where(g => g.CourseId == course.Id)
            .OrderBy(g => g.Id).Select(g => g.Id).FirstOrDefaultAsync();
        
        var group = new Group
        {
            Id = nextId +1,
            Course = course,
            CourseId = course.Id,
            GroupName = name
        };
        var resultGroup = await typeContext.Groups.AddAsync(group);
        await typeContext.SaveChangesAsync();
        return resultGroup.Entity;
    }
    
    /// <summary>
    /// Creates the specified number of groups with the given name prefix,
    /// for instance, with the prefix 'Group', the resulting group name would then be in the range of 'Group 1' - 'Group 50'.
    /// Returns the newly created groups. 
    /// </summary>
    /// <param name="namePrefix">The prefix</param>
    /// <param name="amount">the number of groups to create</param>
    /// <param name="course">the course to create the groups in</param>
    /// <returns>a list of the newly created groups</returns>
    public async Task<List<Group>> CreateNewGroupsAsync(string namePrefix,int amount, Course course)
    {
        var nextId =  await typeContext.Groups
            .Where(g => g.CourseId == course.Id)
            .Select(g => (int?)g.Id)
            .MaxAsync() ?? 0;
        List<Group> groups = new List<Group>(amount);
        for (var i = 0; i < amount; i++)
        {
            nextId++;
            var group = new Group
            {
                Id = nextId,
                Course = course,
                CourseId = course.Id,
                GroupName = $"{namePrefix} {nextId}"
            };
            groups.Add(group);
        }
        await typeContext.Groups.AddRangeAsync(groups);
        await typeContext.SaveChangesAsync();
        return groups;
    }

    /// <summary>
    /// Adds a user to the specified group.
    /// </summary>
    /// <param name="user">the user to add</param>
    /// <param name="groupId">the id of the group</param>
    /// <param name="course">the course</param>
    /// <returns>the group the user was added to</returns>
    /// <exception cref="UserAlreadyExistsException">Thrown if the user already is a part of a group</exception>
    public async Task<Group> AddUserToGroup(GrupperingUser user, int groupId, Course course)
    {
        var group = await typeContext.Groups.Include(g => g.Members).FirstAsync(g => g.Id == groupId && g.CourseId == course.Id);
        if (group.Members.Contains(user))
            throw new UserAlreadyExistsException("User is already a part of this group");
        group.Members.Add(user);
        await typeContext.SaveChangesAsync();
        return group;
    }

    /// <summary>
    /// Removes a user from the specified group
    /// </summary>
    /// <param name="user">the user to remove</param>
    /// <param name="groupId">the id of the group to remove the user from</param>
    /// <param name="course">the course</param>
    /// <returns></returns>
    /// <exception cref="UserNotFoundException">Thrown if the user is not a part of this group</exception>
    public async Task<Group> RemoveUserFromGroup(GrupperingUser user, int groupId, Course course)
    {
        var group = await typeContext.Groups.Include(g => g.Members).FirstAsync(g => g.Id == groupId && g.CourseId == course.Id);
        if (!group.Members.Contains(user))
            throw new UserNotFoundException("User is not a part of this group!");
        group.Members.Remove(user);
        await typeContext.SaveChangesAsync();
        return group;
    }

    public async Task UpdateGroupAsync()
    {
        await typeContext.SaveChangesAsync();
    }

    public async Task UpdateGroupsAsync()
    {
        await typeContext.SaveChangesAsync();
    }

    public async Task DeleteGroupAsync(int groupId, Course course)
    {
        var groupToDelete = typeContext.Groups.First(g => g.Id == groupId && g.CourseId == course.Id);
        typeContext.Groups.Remove(groupToDelete);
        await typeContext.SaveChangesAsync();
    }
}