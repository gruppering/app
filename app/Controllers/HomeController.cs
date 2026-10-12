using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using app.DTO.Auth.Google;
using Microsoft.AspNetCore.Mvc;
using app.Models;
using app.Services;
using app.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace app.Controllers;

public class HomeController: Controller
{
    private HttpClient _client;
    private UserService _userService;

    public HomeController(UserService userService)
    {
        _userService = userService;
        _client = new();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
    }

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
        var user = _userService.GetUserByUsername(username);
        if (user is not null && _userService.VerifyPasswordMatches(user, password))
        {
            List<Claim> claims = user.Roles.ConvertAll(role => new Claim(ClaimTypes.Role, role.Role));
            claims.Add(new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()));
            
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
        try
        {
            var user = await _userService.Signup(username, email, password);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, "ROLE_USER")
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                AuthenticationUtils.GenerateClaimsPrincipal(claims));

            if (HttpContext.Request.Query.TryGetValue("ReturnUrl", out var redirectUrl) && !redirectUrl.Equals(""))
                return Redirect(redirectUrl!);

            return RedirectToAction("Index", "Home");
        }
        catch (ArgumentException)
        {
            
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

    [AllowAnonymous]
    [HttpGet("auth/google")]
    public async Task<IActionResult> GoogleAuthCallback([FromQuery] string code)
    {
        Console.WriteLine(code);
        var body = JsonSerializer.Serialize(
            new GoogleAuthCodeRequestDto(code));
        Console.WriteLine(body);
        var content = new StringContent(body,Encoding.UTF8,"application/json");
        var response = await _client.PostAsync(" https://oauth2.googleapis.com/token", content);

        Console.WriteLine(await response.Content.ReadAsStringAsync());
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        
        Console.WriteLine(authResponse);
        
        var token = new JwtSecurityTokenHandler().ReadJwtToken(authResponse!.id_token);

        var user = await _userService.GoogleLoginOrElseSignup(token);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
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