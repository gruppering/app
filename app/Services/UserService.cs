using System.IdentityModel.Tokens.Jwt;
using app.Models;
using app.Repositories;

namespace app.Services;

public class UserService(IUserRepository userRepository)
{

    public GrupperingUser GetUserByUsername(string username)
    {
        return userRepository.GetUserByUsername(username) ?? throw new ArgumentException("User with this username was not found");
    }
    public GrupperingUser GetUserById(string id)
    {
        return userRepository.GetUserById(id) ?? throw new ArgumentException("User with this id was not found");
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
    public async Task<GrupperingUser> Signup(string username,string email, string password)
    {
        if (userRepository.UserExistsWithUsername(username))
            throw new ArgumentException("User with this username already exists.");

        if (password.Length == 0 || password.IsWhiteSpace())
        {
            throw new ArgumentException("Paasword is empty");
        }
        
        var user = new GrupperingUser
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Roles = [new GrupperingUserRole("ROLE_USER")]
        };
        return await userRepository.SaveAsync(user);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<GrupperingUser> GoogleLoginOrElseSignup(JwtSecurityToken token)
    {
        var existingUser = userRepository.GetUserByIssuerAndSubject(token.Issuer, token.Subject);
        if (existingUser is not null)
            return existingUser;
        
        
        var user = new GrupperingUser
        {
            Username = $"{token.Claims.First(c => c.Type == "given_name").Value}{Random.Shared.Next(9999)}",
            Email = token.Claims.First(c => c.Type == "email").Value,
            ExternalAuth = new ExternalAuth()
            {
                Issuer = token.Issuer,
                Subject = token.Subject
            },
            Roles = [new GrupperingUserRole("ROLE_USER")]
        };
        return await userRepository.SaveAsync(user);
    }


    
    
    /*
     * Creates the specified amount of users with the given name (+ the id), email and password, intended for use in evaluation.
     */
    public async Task<List<GrupperingUser>> CreateRangeOfUsers(string namePrefix,string emailSuffix,string password,int amount)
    {
        var users = new List<GrupperingUser>();
        for (int i = 0; i < amount; i++)
        {
            var user = new GrupperingUser
            {
                Username = namePrefix + i,
                Email = $"{i}@{emailSuffix}",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Roles = [new GrupperingUserRole("ROLE_USER")]
            };
            users.Add(user);
        }
        await userRepository.SaveRangeAsync(users);
        return users;
    }
}