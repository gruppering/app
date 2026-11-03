using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using app.DTO.User;
using app.Services;


namespace app.Controllers;

public class UserController(UserService userService) : Controller // Could remove group service
{
    // Might be unnecessary use of viewmodel dependent on what other info should be on the page 
    [HttpGet("/profile")]
    public async Task<IActionResult> Profile()
    {
        try
        {
            var uId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (uId == null) return BadRequest();
            var user = await userService.GetUserDtoByIdAsync(uId.Value);

            /* TODO Remove these console output statements. */
            /* Inspecting the ClaimTypes class. Development only  */
            Console.WriteLine(".NameIdentifier = " + User.FindFirst(ClaimTypes.NameIdentifier));
            Console.WriteLine(".Name = " + User.FindFirst(ClaimTypes.Name));
            Console.WriteLine(".Role = " + User.FindFirst(ClaimTypes.Role));
            Console.WriteLine("UserDto = " + user);
            
            return View(user);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }
    
    // Update user info functionality I think that front-end checks is sufficient 
        [HttpPost("/user/edit")]
        public async Task<IActionResult> EditUser(UserPageDto dto)
        {
            try
            {
                var uid = User.FindFirst(ClaimTypes.NameIdentifier);
                if (uid == null) return BadRequest();
                
                await userService.UpdateUserInfoAsync(User.FindFirst(ClaimTypes.NameIdentifier)!.Value, dto);
                
                return RedirectToAction("Profile");
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }
        
    // Delete user functionality with the required input of confirming of the action 
    [HttpPost("/user/delete")]
    public async Task<IActionResult> Delete([FromBody] string confirm)
    {
        try{
            if (confirm != "delete") return RedirectToAction("Profile");
            var uId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (uId == null) return BadRequest();
            var user = await userService.GetUserByIdAsync(uId.Value);
            await userService.DeleteUser(user);
            return RedirectToAction("Index", "Home");
        }
        catch(ArgumentException)
        {
            return BadRequest();
        }
    }
}

    
