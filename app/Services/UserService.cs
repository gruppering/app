using app.DAL;
using app.Models;

namespace app.Services;

public class UserService(GrupperingTypeContext typeContext)
{

    /**
     * Verifies if the specified user and password is correct, if it can't find the specified user, it returns false,
     * using this fancy return statement suggested by the IDE.
     */
    public bool Authenticate(string username,string password)
    {
        var user = typeContext.Users.FirstOrDefault(u => u.Username == username);
        return user is not null && BCrypt.Net.BCrypt.Verify(password,user.Password);
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
            Password = BCrypt.Net.BCrypt.HashPassword(password)
        };
        var createdUser = typeContext.Users.Add(user);
        typeContext.SaveChanges();
        return createdUser.Entity;
    }
}