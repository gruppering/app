using app.DAL;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repositories;

public class GroupRepository(GrupperingTypeContext typeContext): IGroupRepository
{

    /**
     * Gets a group by the specified name and course.
     */
    public Group GetGroupByNameAndCourseId(string name, Guid courseId)
    {
        return typeContext.Groups.First(g => g.GroupName == name && g.CourseId == courseId);
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

    public Task<List<Group>> CreateNewGroupsAsync(string namePrefix,int amount, Course course)
    {
        throw new NotImplementedException();
    }

    public Task<Group> AddUserToGroup(GrupperingUser user, int groupId, Course course)
    {
        throw new NotImplementedException();
    }

    public Task<Group> RemoveUserFromGroup(GrupperingUser user, int groupId, Course course)
    {
        throw new NotImplementedException();
    }
}