using app.Models;
using app.Repositories;

namespace app.Services;

public class GroupService(IGroupRepository groupRepository,ICourseRepository courseRepository)
{
    
    /// <summary>
    /// Gets all the groups of a user by their username
    /// </summary>
    /// <param name="userName">the username of the user</param>
    /// <returns>a list of the groups the user is a member of</returns>
    public async Task<List<Group>> GetUserGroups(string userName)
    {
        return await groupRepository.GetUserGroups(userName);
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
        return groupRepository.GetGroupByNameAndCourseId(name, courseId);
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
        return await groupRepository.GetGroupByCourseIdAndId(groupId, courseId);
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
        return await groupRepository.GetGroupsByNamePrefixAndCourseId(namePrefix, courseId);
    }

    /// <summary>
    /// Gets all groups from the specified course.
    /// </summary>
    /// <param name="courseId">The course id</param>
    /// <returns>A list of groups</returns>
    /// <exception cref="CourseNotFoundException">Thrown if the course doesn't exist or if it has no groups</exception>
    public async Task<List<Group>> GetAllGroupsByCourse(Guid courseId)
    {
        return await groupRepository.GetAllGroupsByCourse(courseId);
    }

    /*
     * Delegates the users to groups, based on the existing numbers of groups,
     * if redistribute users is true, it just randomly places them in groups,
     * and if redistribute users is false, adds them to a new group called leftovers,
     * probably rude, but the best solution I could find for now.
     */
    public async Task<List<Group>> DelegateUsersToGroups(Guid courseId,bool redistributeUsers)
    {
        var course = await courseRepository.GetCourseByIdAsync(courseId);
        var groups = await groupRepository.GetAllGroupsByCourse(courseId);
        var users = await courseRepository.GetAllUsersFromCourseByIdAsync(courseId);
        int usersPerGroup = users.Count / groups.Count;
        int stragglers = users.Count % groups.Count;
        
        foreach (var group in groups)
        {
            for (int i = 0; i < usersPerGroup; i++)
            {
                int choice = Random.Shared.Next(0, users.Count); // Untested, need to come back to this.
                group.Members.Add(users[choice]);
                users.RemoveAt(choice);
            }
        }

        if (stragglers != 0 && !redistributeUsers) // Adds any leftover users to a leftover group.
        {
            var stragglerGroup = await groupRepository.CreateNewGroupAsync("Leftovers", course);
            stragglerGroup.Members.AddRange(users);
            groups.Add(stragglerGroup);
        }
        else if (stragglers != 0 && redistributeUsers) // Adds them to random existing groups.
        {
            foreach (var u in users)
            {
                groups[Random.Shared.Next(0,groups.Count)].Members.Add(u);
            }
        }
        await groupRepository.UpdateGroupsAsync();
        return groups;
    }
    
    /*
     * Delegates the users to groups, based on a specified amount of users.
     */
    public async Task<List<Group>> DelegateUsersToGroups(Guid courseId,bool redistributeUsers,int amount,string groupPrefix)
    {
        var course = await courseRepository.GetCourseByIdAsync(courseId);
        var groups = await groupRepository.CreateNewGroupsAsync(groupPrefix,amount, course);
        var users = await courseRepository.GetAllUsersFromCourseByIdAsync(courseId);
        var usersToAdd = users;
        int usersPerGroup = users.Count / amount;
        int stragglers = users.Count % amount;
        
        
        foreach (var group in groups)
        {
            for (int i = 0; i < usersPerGroup; i++)
            {
                int choice = Random.Shared.Next(0, users.Count); // Untested, need to come back to this.
                group.Members.Add(usersToAdd[choice]);
                usersToAdd.RemoveAt(choice);
            }
        }

        if (stragglers != 0 && !redistributeUsers) // Adds any leftover users to a leftover group.
        {
            var stragglerGroup = await groupRepository.CreateNewGroupAsync("Leftovers", course);
            stragglerGroup.Members.AddRange(users);
            groups.Add(stragglerGroup);
        }
        else if (stragglers != 0 && redistributeUsers) // Adds them to random existing groups.
        {
            foreach (var u in users)
            {
                groups[Random.Shared.Next(0,groups.Count)].Members.Add(u);
            }
        }
        await groupRepository.UpdateGroupsAsync();
        return groups;
    }
    
    public async Task<Group> CreateNewGroupAsync(string name, Course course)
    {
        return await groupRepository.CreateNewGroupAsync(name, course);
    }

    public async Task<List<Group>> CreateNewGroupsAsync(string namePrefix, int amount, Course course)
    {
        return await groupRepository.CreateNewGroupsAsync(namePrefix, amount, course);
    }

    /// <summary>
    /// Adds a user to a group
    /// </summary>
    /// <param name="user">The user to add</param>
    /// <param name="groupId">the group id to add the user to</param>
    /// <param name="course">the course associated with the group</param>
    /// <exception cref="UserAlreadyExistsException">Thrown by the underlying repository method if a user is already a part of a group</exception>
    /// <returns>The group the user was added to</returns>
    public async Task<Group> AddUserToGroup(GrupperingUser user, int groupId, Course course)
    {
        return await groupRepository.AddUserToGroup(user, groupId, course);
    }

    /// <summary>
    /// Removes a user frop a group
    /// </summary>
    /// <param name="user">The user to remove</param>
    /// <param name="groupId">the group id to remove the user from</param>
    /// <param name="course">the course associated with the group</param>
    /// <exception cref="UserNotFoundException">Thrown by the underlying repository method if a user isn't found in the group</exception>
    /// <returns>The group the user was removed from</returns>
    public async Task<Group> RemoveUserFromGroup(GrupperingUser user, int groupId, Course course)
    {
        return await groupRepository.RemoveUserFromGroup(user, groupId, course);
    }
}