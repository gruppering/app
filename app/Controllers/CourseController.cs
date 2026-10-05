using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using app.DTO.Course;
using app.Services;
using app.ViewModel;

namespace app.Controllers;
public class CourseController(CourseService courseService,GroupService groupService, UserService userService) : Controller
{
        // Sends all PUBLIC courses to the endpoint NOTE; add Paging if time 
    [HttpGet("/courses")]
    public async Task<IActionResult> Courses()
    {
        try
        {
            var publicCourses = await courseService.GetAllPublicCoursesAsync();

            var uName = User.FindFirst(ClaimTypes.Name)!.Value;
            var user = userService.GetUserByUsername(uName);
            var memberOfCourse = await courseService.GetAllCoursesWithMemberAsync(user);
            Console.WriteLine(memberOfCourse.Count);
            var courseViewModel = new CourseViewModel(publicCourses, memberOfCourse);
            return View(courseViewModel);
        }
        catch (ArgumentException)
        {
            return RedirectToAction("Error","Home",new {code = 404,description="Could not find courses"});
        }
    }

        // Gets the course that is called and sends the user to a descriptive page of the course
    [HttpGet("/courses/{courseName}")]
    public async Task<IActionResult> Details(string courseName)
    {
        try
        {
            var course = await courseService.GetCourseByNameAsync(courseName);
            // Checking if the current user based on name is the same as the creator of a course
            // This can be used to make the course details content differ depending on ownership
            var isOwner = course.Owner.Username == User.FindFirst(ClaimTypes.Name)!.Value;
            var users = await courseService.GetAllUsersFromCourseAsync(courseName);
            var groups = await groupService.GetAllGroupsByCourse(course.Id);
            var detailCourseViewModel = new CourseDetailViewModel(course, isOwner, users,groups);
        
            return View(detailCourseViewModel);
        }
        catch(ArgumentException)
        {
            return RedirectToAction("Error","Home",new {code = 404,description="A Course with that name was not found"});
        }
    }
    
    [HttpGet("/courses/join/{joinCode}")]
    public async Task<IActionResult> Join(string joinCode)
    {
        try
        {
            var course = await courseService.GetCourseByJoinCodeAsync(joinCode);
            // Checking if the current user based on name is the same as the creator of a course
            // This can be used to make the course details content differ depending on ownership
            var isOwner = course.Owner.Username == User.FindFirst(ClaimTypes.Name)!.Value;
            if (isOwner) return RedirectToAction("Error", "Home",new {code=500,description="You are the owner of this course"});
            await courseService.AddUserToCourseAsync(course.DisplayName, User.FindFirst(ClaimTypes.Name)!.Value);
            
            return Redirect($"../{course.DisplayName}"); // This is extremely hacky, but I am tired, so I'll let it be for now.
        }
        catch(ArgumentException)
        {
            return RedirectToAction("Error","Home",new {code = 404,description="Could not create join code"});
        }
    }
    
    [HttpGet("/courses/{courseName}/join")]
    public async Task<IActionResult> JoinCourse(string courseName)
    {
        try
        {
            var course = await courseService.GetCourseByNameAsync(courseName);
            // Checking if the current user based on name is the same as the creator of a course
            // This can be used to make the course details content differ depending on ownership
            var isOwner = course.Owner.Username == User.FindFirst(ClaimTypes.Name)!.Value;
            if (isOwner) return RedirectToAction("Error", "Home",new {code=500,description="You are the owner of this course"});
            await courseService.AddUserToCourseAsync(course.DisplayName, User.FindFirst(ClaimTypes.Name)!.Value);
            
            return Redirect($"../{course.DisplayName}"); // This is extremely hacky, but I am tired, so I'll let it be for now.
        }
        catch(ArgumentException)
        {
            return RedirectToAction("Error","Home",new {code = 404,description="Could not join the course"});
        }
    }
    
        // Create course NOTE; this is not implemented yet cause idk where it should be added 
    [HttpPost("/courses/create")]
    public async Task<IActionResult> CreateCourse(CourseCreationDto dto)
    {
        try
        {
            var uName = User.FindFirst(ClaimTypes.Name)!.Value;
            var creator = userService.GetUserByUsername(uName);

            await courseService.CreateNewCourseAsync(dto, creator);

            return RedirectToAction("Courses");
        }
        catch (ArgumentException)
        {
            return RedirectToAction("Error","Home",new {code = 404,description="Could not create the course"});
        }
    }
}