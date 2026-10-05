using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using app.Models;
using app.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace app.Controllers;

public class HomeController(UserService userService) : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login() => View();

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login( [FromForm]string username, [FromForm]string password)
    {
        var user = userService.GetUserByUsername(username);
        if (user is not null && userService.VerifyPasswordMatches(user, password))
        {
            List<Claim> claims = user.Roles.ConvertAll(role => new Claim(ClaimTypes.Role, role.Role));
            claims.Add(new Claim(ClaimTypes.Name,user.Username));
            var claimsIdentity = new ClaimsIdentity(claims, "cookie");
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            
            if (HttpContext.Request.Query.TryGetValue("ReturnUrl",out var redirectUrl) && !redirectUrl.Equals(""))
                return Redirect(redirectUrl!);
            
            return RedirectToAction("Index", "Home");
        }
        ViewBag.Error = "Invalid username or password";
        return View();
    }
    
    [HttpGet("signup")]
    [AllowAnonymous]
    public IActionResult Signup() => View();

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> Signup( [FromForm]string username,[FromForm] string email, [FromForm]string password)
    {
        if (await userService.Signup(username, email, password) is not null)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, username),
                new (ClaimTypes.Role, "ROLE_USER")
            };
            var claimsIdentity = new ClaimsIdentity(claims, "cookie");
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            
            if (HttpContext.Request.Query.TryGetValue("ReturnUrl",out var redirectUrl) && !redirectUrl.Equals(""))
                return Redirect(redirectUrl!);
            
            return RedirectToAction("Index", "Home");
        }
        ViewBag.Error = "User with this username already exists!";
        return View();
    }
    
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Home");
    }

    

    [HttpGet("error/{code}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int code,[FromQuery] string description)
    {
        return View(new ErrorViewModel {ErrorDescription = description,ErrorCode = code, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}