using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using app.DTO.Course;
using app.Services;
using app.ViewModel;

namespace app.Controllers;
public class CourseController(CourseService courseService,GroupService groupService, UserService userService) : Controller
{
    private static readonly ILoggerFactory Factory = LoggerFactory.Create(builder => builder.AddConsole());
    private readonly ILogger _logger = Factory.CreateLogger("Groups");
    
    // Sends all PUBLIC courses to the endpoint NOTE; add Paging if time 
    [HttpGet("/courses")]
    public async Task<IActionResult> Courses()
    {
        try
        {
            var publicCourses = await courseService.GetAllPublicCoursesAsync();

            var uId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = userService.GetUserById(uId);
            var memberOfCourse = await courseService.GetAllCoursesWithMemberAsync(user);
            var courseViewModel = new CourseViewModel(publicCourses, memberOfCourse);
            return View(courseViewModel);
        }
        catch (ArgumentException)
        {
            return RedirectToAction("Error","Home",new {code = 404,description="Could not find courses"});
        }
    }

    // Gets the course that is called and sends the user to a descriptive page of the course
    [HttpGet("/course")]
    public async Task<IActionResult> Details([FromQuery] string courseName)
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
            _logger.LogError("No course with name: {courseName} was found",courseName);
            return RedirectToAction("Error","Home",new {code = 404,description="A Course with that name was not found"});
        }
    }
    
    [HttpGet("/courses/join")]
    public async Task<IActionResult> Join([FromQuery] string joinCode)
    {
        try
        {
            var course = await courseService.GetCourseByJoinCodeAsync(joinCode);
            // Checking if the current user based on name is the same as the creator of a course
            // This can be used to make the course details content differ depending on ownership
            var isOwner = course.Owner.Username == User.FindFirst(ClaimTypes.Name)!.Value;
            if (isOwner) return RedirectToAction("Error", "Home",new {code=500,description="You are the owner of this course"});
            await courseService.AddUserToCourseAsync(course.DisplayName, User.FindFirst(ClaimTypes.Name)!.Value);
            
            return RedirectToAction("Details","Course",new {courseName=course.DisplayName}); // This is extremely hacky, but I am tired, so I'll let it be for now.
        }
        catch(ArgumentException)
        {
            _logger.LogError("No course with join code: {joinCode} was found",joinCode);
            return RedirectToAction("Error","Home",new {code = 404,description="Could not find course with join code"});
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
            _logger.LogError("Could not join the course");
            return RedirectToAction("Error","Home",new {code = 404,description="Could not join the course"});
        }
    }
    
    [HttpPost("/courses/create")]
    public async Task<IActionResult> CreateCourse(CourseCreationDto dto)
    {
        try
        {
            var uId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var creator = userService.GetUserById(uId);

            await courseService.CreateNewCourseAsync(dto, creator);

            return RedirectToAction("Courses");
        }
        catch (ArgumentException)
        {
            _logger.LogError("Could not create course with params; Name: {DtoCourseName}, Description: {DtoDescription},Is Public: {DtoIsPublic}", dto.CourseName, dto.Description, dto.IsPublic);

            return RedirectToAction("Error","Home",new {code = 404,description="Could not create the course"});
        }
    }
}