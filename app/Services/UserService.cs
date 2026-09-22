using app.DAL;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Services;

public class UserService(GrupperingTypeContext typeContext)
{

    public GrupperingUser? GetUserByUsername(string username)
    {
        var user = typeContext.Users.Include(u => u.Roles).FirstOrDefault(u => u.Username == username);
        return user;
    }
    
    /**
     * Verifies if the password matches the password with the supplied user.
     */
    public bool VerifyPasswordMatches(GrupperingUser user,string passwordToCheck)
    {
        return BCrypt.Net.BCrypt.Verify(passwordToCheck,user.Password);
    }

    /**
     * Creates a new user and adds them to the db, returns null if username already exists.
     */
    public GrupperingUser? Signup(string username,string email, string password)
    {
        if (typeContext.Users.Any(u => u.Username == username))
            return null;
        
        var user = new GrupperingUser
        {
            Username = username,
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Roles = [new GrupperingUserRole("ROLE_USER")]
        };
        var createdUser = typeContext.Users.Add(user);
        typeContext.SaveChanges();
        return createdUser.Entity;
    }
}