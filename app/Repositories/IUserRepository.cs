using app.Models;

namespace app.Repositories;

public interface IUserRepository
{ 
    GrupperingUser? GetUserByUsername(string username);
    bool UserExistsWithUsername(string username);

    Task<GrupperingUser> SaveAsync(GrupperingUser user);
    Task SaveRangeAsync(List<GrupperingUser> user);

}