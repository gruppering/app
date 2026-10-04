using app.DAL;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repositories;

public class UserRepository(GrupperingTypeContext typeContext): IUserRepository
{
    public GrupperingUser? GetUserByUsername(string username)
    {
        return typeContext.Users.Include(u => u.Roles).FirstOrDefault(u => u.Username == username);
    }

    public bool UserExistsWithUsername(string username)
    {
        return typeContext.Users.Any(u => u.Username == username);
    }

    public async Task<GrupperingUser> SaveAsync(GrupperingUser user)
    {
        var createdUser = await typeContext.Users.AddAsync(user);
        await typeContext.SaveChangesAsync();
        return createdUser.Entity;
    }

    public async Task SaveRangeAsync(List<GrupperingUser> users)
    {
        await typeContext.Users.AddRangeAsync(users);
        await typeContext.SaveChangesAsync();
    }
}