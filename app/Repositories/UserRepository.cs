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
    
    public GrupperingUser? GetUserById(string id)
    {
        return typeContext.Users.Include(u => u.Roles).FirstOrDefault(u => u.Id.ToString() == id);
    }
    
    public async Task<GrupperingUser?> GetUserByIdAsync(string id)
    {
        return await typeContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id.ToString() == id);
    }



    public bool UserExistsWithUsername(string username)
    {
        return typeContext.Users.Any(u => u.Username == username);
    }

    /// <summary>
    /// Checks if user has signed in previously with this external account.
    /// </summary>
    /// <param name="issuer"></param>
    /// <param name="subject"></param>
    /// <returns></returns>
    public GrupperingUser? GetUserByIssuerAndSubject(string issuer, string subject)
    {
        return typeContext.Users.Include(gu => gu.Roles).Include(gu => gu.ExternalAuth)
            .FirstOrDefault(gu => gu.ExternalAuth!.Issuer == issuer && gu.ExternalAuth.Subject == subject);
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
    
    public async Task DeleteUser(GrupperingUser user)
    {
        typeContext.Users.Remove(user);
        await typeContext.SaveChangesAsync();
    }
    
    public async Task UpdateUser(GrupperingUser user)
    {
        typeContext.Users.Update(user);
        await typeContext.SaveChangesAsync();
    }
}