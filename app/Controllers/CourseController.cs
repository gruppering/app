using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using app.DTO.Course;
using app.Services;
using app.ViewModel;
using app.Models;

namespace app.Controllers;
public class CourseController(CourseService courseService, UserService userService) : Controller
{
        // Sends all PUBLIC courses to the endpoint NOTE; add Paging if time 
    [HttpGet("/courses")]
    public async Task<IActionResult> Courses()
    {
        var publicCourses = await courseService.GetAllPublicCoursesAsync();
        
        var uName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = userService.GetUserByUsername(uName)!;
        var memberOfCourse = await courseService.GetAllCoursesWithMemberAsync(user);
        
        var courseViewModel = new CourseViewModel(publicCourses, memberOfCourse);
        return View(courseViewModel);
    }

        // Gets the course that is called and sends the user to a descriptive page of the course
    [HttpGet("/courses/{courseName}")]
    public async Task<IActionResult> Details(string courseName)
    {
        var course = await courseService.GetCourseByNameAsync(courseName);
        if (course is null)
                    return NotFound();
        
        // Checking if the current user based on name is the same as the creator of a course
        // This can be used to make the course details content differ depending on ownership
        var isOwner = course.Owner.Username == User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var users = await courseService.GetAllUsersFromCourseAsync(courseName);
        
        var detailCourseViewModel = new CourseDetailViewModel(course, isOwner, users);
        
        return View(detailCourseViewModel);
    }
    
        // Create course NOTE; this is not implemented yet cause idk where it should be added 
    [HttpPost("/courses/create")]
    public async Task<IActionResult> CreateCourse(CourseCreationDto dto)
    {
        var uName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var creator = userService.GetUserByUsername(uName)!;
        
        await courseService.CreateNewCourseAsync(dto,creator);
        
        return RedirectToAction("Courses");
    }
}