using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using app.DTO;
using app.DTO.User;
using app.ViewModel;
using app.Services;


namespace app.Controllers;

public class ProfileController(UserService userService, CourseService courseService, GroupService groupService) : Controller // Could remove group service
{
    [HttpGet("/profile")]
    public async Task<IActionResult> Profile()
    {
        var uId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var user = userService.GetUserByIdDTO(uId);
        
        var viewModel = new UserProfileModel(user);
        return View(viewModel);
    }
}

    
