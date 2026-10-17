using app.Models;

namespace app.Repositories;

public interface IUserRepository
{ 
    GrupperingUser? GetUserByUsername(string username);
    GrupperingUser? GetUserById(string id);

    bool UserExistsWithUsername(string username);
    GrupperingUser? GetUserByIssuerAndSubject(string issuer,string subject);
    
    Task<GrupperingUser> SaveAsync(GrupperingUser user);
    Task SaveRangeAsync(List<GrupperingUser> user);
    Task DeleteUser(GrupperingUser user);
    
    Task UpdateUser(GrupperingUser user);

}