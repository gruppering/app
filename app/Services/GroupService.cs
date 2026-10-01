using app.Models;
using app.Repositories;

namespace app.Services;

public class GroupService(IGroupRepository groupRepository)
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