using app.Models;
using app.Repositories;

namespace app.Services;

public class UserService(IUserRepository userRepository)
{

    public GrupperingUser? GetUserByUsername(string username)
    {
        return userRepository.GetUserByUsername(username);
    }
    
    /**
     * Verifies if the password matches the password with the supplied user.
     */
    public bool VerifyPasswordMatches(GrupperingUser user,string passwordToCheck)
    {
        return BCrypt.Net.BCrypt.Verify(passwordToCheck,user.PasswordHash);
    }

    /**
     * Creates a new user and adds them to the db, returns null if username already exists.
     */
    public async Task<GrupperingUser?>? Signup(string username,string email, string password)
    {
        if (userRepository.UserExistsWithUsername(username))
            return null;
        
        var user = new GrupperingUser
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Roles = [new GrupperingUserRole("ROLE_USER")]
        };
        
        return await userRepository.Save(user);
    }
}