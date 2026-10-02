using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using app.DTO.Course;
using app.Services;
using app.ViewModel;

namespace app.Controllers;
public class CourseController(CourseService courseService, UserService userService) : Controller
{
        // Sends all PUBLIC courses to the endpoint NOTE; add Paging if time 
    [HttpGet("/courses")]
    public async Task<IActionResult> Courses()
    {
        var course = await courseService.GetAllPublicCoursesAsync();
        return View(course);
    }

        // Gets the course that is called and sends the user to a descriptive page of the course
    [HttpGet("/courses/{courseName}")]
    public async Task<IActionResult> Details(string courseName)
    {
        var course = await courseService.GetCourseByNameAsync(courseName);
        
        if (course is null)
            return NotFound();
        
        return View(course);
    }
    
        // Create course NOTE; this is not implemented yet cause idk where it should be added 
    [HttpPost("")]
    public async Task<IActionResult> CreateCourse(CourseCreationDto dto)
    {
        var uName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var creator = userService.GetUserByUsername(uName)!;
            
        await courseService.CreateNewCourse(dto,creator);
        
        return RedirectToAction();
    }
}