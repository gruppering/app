using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using app.DTO.User;
using app.ViewModel;
using app.Services;


namespace app.Controllers;

public class UserController(UserService userService, CourseService courseService, GroupService groupService) : Controller // Could remove group service
{
    // Might be unnecessary use of viewmodel dependent on what other info should be on the page 
    [HttpGet("/profile")]
    public async Task<IActionResult> Profile()
    {
        var uId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var user = userService.GetUserByIdDto(uId);
        
        var viewModel = new UserProfileModel(user);
        return View(viewModel);
    }
    
    // Delete user functionality with the required input of confirming of the action 
    [HttpPost("/user/delete")]
    public async Task<IActionResult> Delete([FromBody] string confirm)
    {
        if (confirm != "delete") return RedirectToAction("Profile");
        var uId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var user = userService.GetUserById(uId);
        try
        {
            await userService.DeleteUser(user);
        }
        catch(ArgumentException)
        {
            return RedirectToAction("Error","Home",new {code = 404,description="Could not delete user"});
        }
        return RedirectToAction("Index", "Home");
    }

    // Update user info functionality i think that front-end checks is sufficient 
    [HttpPost("/user/edit")]
    public async Task<IActionResult> Edit(UserPageDto dto)
    {
        try
        {
            await userService.UpdateUserInfoAsync(User.FindFirst(ClaimTypes.NameIdentifier)!.Value, dto);
            return RedirectToAction("Profile");
        }
        catch (ArgumentException)
        {
            return RedirectToAction("Error","Home",new {code = 404,description="Could not update user information"});
        }
    }
}

    
