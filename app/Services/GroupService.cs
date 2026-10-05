using app.Models;
using app.Repositories;

namespace app.Services;

public class GroupService(IGroupRepository groupRepository,ICourseRepository courseRepository)
{
    
    public async Task<List<Group>> GetUserGroups(string userName)
    {
        return await groupRepository.GetUserGroups(userName);
    }

    
    public Group GetGroupByNameAndCourseId(string name, Guid courseId)
    {
        return groupRepository.GetGroupByNameAndCourseId(name, courseId);
    }

    public async Task<Group> GetGroupByCourseIdAndId(int groupId, Guid courseId)
    {
        return await groupRepository.GetGroupByCourseIdAndId(groupId, courseId);
    }

    public async Task<List<Group>> GetGroupsByNamePrefixAndCourseId(string namePrefix, Guid courseId)
    {
        return await groupRepository.GetGroupsByNamePrefixAndCourseId(namePrefix, courseId);
    }

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
        var users = await courseRepository.GetAllUsersFromCourseById(courseId);
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
        await groupRepository.UpdateGroupsAsync(groups);
        return groups;
    }
    
    /*
     * Delegates the users to groups, based on a specified amount of users.
     */
    public async Task<List<Group>> DelegateUsersToGroups(Guid courseId,bool redistributeUsers,int amount,string groupPrefix)
    {
        var course = await courseRepository.GetCourseByIdAsync(courseId);
        var groups = await groupRepository.CreateNewGroupsAsync(groupPrefix,amount, course);
        var users = await courseRepository.GetAllUsersFromCourseById(courseId);
        int usersPerGroup = users.Count / amount;
        int stragglers = users.Count % amount;
        
        
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
        await groupRepository.UpdateGroupsAsync(groups);
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

    public async Task<Group> AddUserToGroup(GrupperingUser user, int groupId, Course course)
    {
        return await groupRepository.AddUserToGroup(user, groupId, course);
    }

    public async Task<Group> RemoveUserFromGroup(GrupperingUser user, int groupId, Course course)
    {
        return await groupRepository.RemoveUserFromGroup(user, groupId, course);
    }
}