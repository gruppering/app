using app.Models;

namespace app.Repositories;

public interface IGroupRepository
{
    public Task<List<Group>> GetUserGroups(string userName);
    
    public Group GetGroupByNameAndCourseId(string name,Guid courseId);
    public Task<Group> GetGroupByCourseIdAndId(int groupId, Guid courseId);
    
    public Task<List<Group>> GetGroupsByNamePrefixAndCourseId(string namePrefix,Guid courseId);
    public Task<List<Group>> GetAllGroupsByCourse(Guid courseId);
    
    public Task<Group> CreateNewGroupAsync(string name,Course course);
    public Task<List<Group>> CreateNewGroupsAsync(string namePrefix,int amount,Course course);

    public Task<Group> AddUserToGroup(GrupperingUser user,int groupId,Course course);
    public Task<Group> RemoveUserFromGroup(GrupperingUser user,int groupId,Course course);
    
}