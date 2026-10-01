using app.DAL;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repositories;

public class GroupRepository(GrupperingTypeContext typeContext): IGroupRepository
{
    
    public async Task<List<Group>> GetUserGroups(string userName)
    {
        return await typeContext.Groups.Include(g => g.Members)
            .Where(g => g.Members.All(gu => gu.Username == userName ))
            .ToListAsync();
    }

    /**
     * Gets a group by the specified name and course.
     */
    public Group GetGroupByNameAndCourseId(string name, Guid courseId)
    {
        return typeContext.Groups.Include(g => g.Members).First(g => g.GroupName == name && g.CourseId == courseId);
    }
    
    /**
     * Gets all groups by the given prefix in a specific course.
     */
    public async Task<List<Group>> GetGroupsByNamePrefixAndCourseId(string namePrefix, Guid courseId)
    {
        return await typeContext.Groups.Where(g => g.GroupName.StartsWith(namePrefix) && g.CourseId == courseId).ToListAsync();
    }

    /**
     * Self-explanatory...
     */
    public async Task<Group> GetGroupByCourseIdAndId(int groupId, Guid courseId)
    {
        return await typeContext.Groups.FirstAsync(g => g.Id == groupId && g.CourseId == courseId);
    }

    /**
     * Self-explanatory...
     */
    public async Task<List<Group>> GetAllGroupsByCourse(Guid courseId)
    {
        return await typeContext.Groups.Where(g => g.CourseId == courseId).ToListAsync();
    }

    /**
     * Creates a new group with the given name, in the specified course.
     * As names are not required to be unique.
     */
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

    /**
     * Creates the specified number of groups with the given name prefix,
     * for instance, with the prefix 'Group', the resulting group name would then be in the range of 'Group 1' - 'Group 50'.
     * Returns the newly created groups.
     */
    public async Task<List<Group>> CreateNewGroupsAsync(string namePrefix,int amount, Course course)
    {
        var nextId =  await typeContext.Groups
            .Where(g => g.CourseId == course.Id)
            .OrderBy(g => g.Id).Select(g => g.Id).FirstOrDefaultAsync();
        List<Group> groups = new List<Group>(amount);
        for (var i = 0; i < amount; i++)
        {
            nextId += 1;
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

    /**
     * Adds the specified user to the specified group,
     * throws an ArgumentException if user is already a part of the group.
     */
    public async Task<Group> AddUserToGroup(GrupperingUser user, int groupId, Course course)
    {
        var group = await typeContext.Groups.Include(g => g.Members).FirstAsync(g => g.Id == groupId && g.CourseId == course.Id);
        if (group.Members.Contains(user))
            throw new ArgumentException("User is already a part of this group");
        group.Members.Add(user);
        await typeContext.SaveChangesAsync();
        return group;
    }

    /**
     * Removes the specified user from the specified group,
     * throws an ArgumentException if user isn't part of the group.
     */
    public async Task<Group> RemoveUserFromGroup(GrupperingUser user, int groupId, Course course)
    {
        var group = await typeContext.Groups.Include(g => g.Members).FirstAsync(g => g.Id == groupId && g.CourseId == course.Id);
        if (!group.Members.Contains(user))
            throw new ArgumentException("User is not a part of this group!");
        group.Members.Remove(user);
        await typeContext.SaveChangesAsync();
        return group;
    }
}