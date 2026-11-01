using System.Diagnostics;
using System.Security.Claims;
using app.Exceptions;
using Microsoft.AspNetCore.Mvc;
using app.Models;
using app.Services;
using app.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.VisualBasic;

namespace app.Controllers;

public class HomeController(UserService userService, GroupService groupService): Controller
{
    private static readonly ILoggerFactory Factory = LoggerFactory.Create(builder => builder.AddConsole());
    private readonly ILogger _logger = Factory.CreateLogger("General");
    
    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        try
        {   
            var uId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (uId == null) return BadRequest();
            var username = await userService.GetUserByIdAsync(uId.Value);
            var groups = await groupService.GetUserGroups(username.Username);
            return View(groups);
        }
        catch (UserNotFoundException)
        {
            return BadRequest();
        }
    }
    
    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login() => View();

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login( [FromForm]string username, [FromForm]string password)
    {
        try
        {
            var user = userService.GetUserByUsername(username);
            if (userService.VerifyPasswordMatches(user, password))
            {
                List<Claim> claims = user.Roles.ConvertAll(role => new Claim(ClaimTypes.Role, role.Role));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, user.Username));
                var claimsIdentity = new ClaimsIdentity(claims, "cookie");
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                if (HttpContext.Request.Query.TryGetValue("ReturnUrl", out var redirectUrl) && !redirectUrl.Equals(""))
                    return Redirect(redirectUrl!);

                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Incorrect username or password";
            return View();
        }
        catch (UserNotFoundException)
        {
            ViewBag.Error = "Incorrect username or password";
            return View();
        }
        catch (Exception e)
        {
            _logger.LogError("Something went terribly wrong; {errorMessage} - {errorLocation}",e.Message,e.TargetSite);
            return RedirectToAction("Error","Home",new {code = 500,description="Something went wrong."});
        }
        
    }
    
    [HttpGet("signup")]
    [AllowAnonymous]
    public IActionResult Signup() => View();

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> Signup( [FromForm]string username,[FromForm] string email, [FromForm]string password)
    {
        try
        {
            var user = await userService.Signup(username, email, password);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, "ROLE_USER")
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                AuthenticationUtils.GenerateClaimsPrincipal(claims));

            if (HttpContext.Request.Query.TryGetValue("ReturnUrl", out var redirectUrl) && !redirectUrl.Equals(""))
                return Redirect(redirectUrl!);

            return RedirectToAction("Index", "Home");
        }
        catch (UserAlreadyExistsException uae)
        {
            ViewBag.Error = uae.Message;
            return View();
        }
        catch (IllegalArgumentException ia) // Handles both an invalid username or password
        {
            ViewBag.Error = ia.Message;
            return View();
        }
    }
    
    
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Home");
    }

    [FeatureGate("GoogleSignin")]
    [AllowAnonymous]
    [HttpGet("auth/google")]
    public async Task<IActionResult> GoogleAuthCallback([FromQuery] string code)
    {
        var token = await AuthenticationUtils.GetGoogleJwt(code,
            $"{Request.Scheme}://{Request.Host}{Request.PathBase}/auth/google");

        var user = await userService.GoogleLoginOrElseSignup(token);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name,user.Username),
            new(ClaimTypes.Role, "ROLE_USER")
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            AuthenticationUtils.GenerateClaimsPrincipal(claims));

        if (HttpContext.Request.Query.TryGetValue("state", out var redirectUrl) && !redirectUrl.Equals(""))
            return Redirect(redirectUrl!);

        return RedirectToAction("Index", "Home");
    }
    
    [FeatureGate("GitHubSignin")]
    [AllowAnonymous]
    [HttpGet("auth/github")]
    public async Task<IActionResult> GitHubAuthCallback([FromQuery] string code)
    {
        var userInformation = await AuthenticationUtils.GetGitHubUserInformation(code,
            $"{Request.Scheme}://{Request.Host}{Request.PathBase}/auth/github");

        var user = await userService.GithubLoginOrElseSignup(userInformation);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name,user.Username),
            new(ClaimTypes.Role, "ROLE_USER")
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            AuthenticationUtils.GenerateClaimsPrincipal(claims));

        if (HttpContext.Request.Query.TryGetValue("state", out var redirectUrl) && !redirectUrl.Equals(""))
            return Redirect(redirectUrl!);

        return RedirectToAction("Index", "Home");
    }

    
    

    [AllowAnonymous]
    [HttpGet("error/{code}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int code,[FromQuery] string description)
    {
        return View(new ErrorViewModel {ErrorDescription = description,ErrorCode = code, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}