using System.IdentityModel.Tokens.Jwt;
using app.DTO.Auth.GitHub;
using app.DTO.User;
using app.Exceptions;
using app.Models;
using app.Repositories;

namespace app.Services;

public class UserService(IUserRepository userRepository)
{

    /// <summary>
    /// Gets a user by their username
    /// </summary>
    /// <param name="username">The username</param>
    /// <returns></returns>
    /// <exception cref="UserNotFoundException">Thrown if the specified user wasn't found</exception>
    public GrupperingUser GetUserByUsername(string username)
    {
        return userRepository.GetUserByUsername(username) ?? throw new UserNotFoundException("User with this username was not found");
    }
    
    /// <summary>
    /// Gets a user by their id
    /// </summary>
    /// <param name="id">The id of the user</param>
    /// <returns></returns>
    /// <exception cref="UserNotFoundException">Thrown if the specified user wasn't found</exception>
    public GrupperingUser GetUserById(string id)
    {
        return userRepository.GetUserById(id) ?? throw new UserNotFoundException("User with this id was not found");
    }
    
    /// <summary>
    /// Gets a user by their id
    /// </summary>
    /// <param name="id">The id of the user</param>
    /// <returns></returns>
    /// <exception cref="UserNotFoundException">Thrown if the specified user wasn't found</exception>
    public async Task<GrupperingUser> GetUserByIdAsync(string id)
    {
        return await userRepository.GetUserByIdAsync(id) ?? throw new UserNotFoundException("User with this id was not found");
    }

    // This method returns a DTO check app/DTO/user for the DTO constructor
    public UserPageDto GetUserByIdDto(string id)
    {
        var user = userRepository.GetUserById(id) ?? throw new ArgumentException("User with this id was not found");
        var userPageDto = new UserPageDto(user.Username, user.Email);
        return userPageDto;
    }
    
    /// <summary>
    /// Verifies the supplied password matches the password from the supplied user.
    /// </summary>
    /// <param name="user">The user object to get the password hash from</param>
    /// <param name="passwordToCheck">the password to compare</param>
    /// <returns>A bool representing whether the password matched</returns>
    public bool VerifyPasswordMatches(GrupperingUser user,string passwordToCheck)
    {
        return BCrypt.Net.BCrypt.Verify(passwordToCheck,user.PasswordHash);
    }

    /// <summary>
    /// Creates a user with the given required parameters.
    /// </summary>
    /// <param name="username">the username of the new user</param>
    /// <param name="email">the email of the new user</param>
    /// <param name="password">the password of the new user</param>
    /// <returns>The created user</returns>
    /// <exception cref="ArgumentException">Thrown if the username is empty</exception>
    /// <exception cref="UserNotFoundException">Thrown if a user with this username already exists</exception>
    /// <exception cref="IllegalArgumentException">Thrown if the password or username does not meet signup criteria</exception>
    public async Task<GrupperingUser> Signup(string username,string email, string password)
    {
        if (username.IsWhiteSpace())
            throw new IllegalArgumentException("Username is an empty string");
        
        if (userRepository.UserExistsWithUsername(username))
            throw new UserAlreadyExistsException("User with this username already exists.");

        if (password.Length == 0 || password.IsWhiteSpace())
        {
            throw new IllegalArgumentException("Password is an empty string");
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
    /// With the given JWT, assuming it contains issuer and subject fields, either creates or logs in a user.
    /// All the required info we need is already present in the jwt, so there is no need to call the endpoints to fetch user info.
    /// </summary>
    /// <param name="token">The JWTSecurityToken acquired from Google.</param>
    /// <returns></returns>
    public async Task<GrupperingUser> GoogleLoginOrElseSignup(JwtSecurityToken token)
    {
        var existingUser = userRepository.GetUserByIssuerAndSubject(token.Issuer, token.Subject);
        if (existingUser is not null)
            return existingUser;
        
        
        var user = new GrupperingUser
        {
            Username = $"{token.Claims.First(c => c.Type == "given_name").Value}{Random.Shared.Next(1000,9999)}",
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

    /// <summary>
    /// With the supplied user info DTO, either creates or logs in a user.
    /// As GitHub doesn't supply a JWT we can extract the info from,
    /// a dedicated dto created from GetGitHubUserInformation in AuthUtils is needed.
    /// </summary>
    /// <param name="userInfoDto">A user info dto, containing the necessary fields to create or identify an already existing account</param>
    /// <returns>A GrupperingUser object, either from an existing user, or the newly created one.</returns>
    public async Task<GrupperingUser> GithubLoginOrElseSignup(GithubUserInfoDto userInfoDto)
    {
        var existingUser = userRepository.GetUserByIssuerAndSubject("https://github.com", $"{userInfoDto.id}");
        if (existingUser is not null)
            return existingUser;
        
        
        var user = new GrupperingUser
        {
            Username = $"{userInfoDto.name.Split(" ")[0]}{Random.Shared.Next(1000,9999)}",
            Email = userInfoDto.email,
            ExternalAuth = new ExternalAuth()
            {
                Issuer = "https://github.com",
                Subject = $"{userInfoDto.id}"
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
    // Deleting user and returning true or false depending on success or not
    // TODO Create Unit tests 
    public async Task<bool> DeleteUser(GrupperingUser? user)
    {
        if (user != null &&  userRepository.UserExistsWithUsername(user.Username))
        {
            await userRepository.DeleteUser(user); 
            return true;
        }
        return false;
    }   
    
        // Updates the info and the user does have to exist in the DB
    public async Task UpdateUserInfoAsync(string userId,UserPageDto dto)
    {
        var user = userRepository.GetUserById(userId);
        if (user != null &&  userRepository.UserExistsWithUsername(user.Username))
        {
            user.Username = dto.Username;
            user.Email = dto.Email;
            await userRepository.UpdateUser(user);
        }
    }
}