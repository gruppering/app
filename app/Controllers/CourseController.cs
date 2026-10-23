using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using app.DTO.Course;
using app.Services;
using app.Utils;
using app.ViewModel;
using QRCoder;

namespace app.Controllers;
public class CourseController(CourseService courseService,GroupService groupService, UserService userService,ChatMessageService chatMessageService) : Controller
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
            var user = await userService.GetUserByIdAsync(uId);
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
    public async Task<IActionResult> Details([FromQuery] string code)
    {
        try
        {
            var course = await courseService.GetCourseByJoinCodeAsync(code);
            // Checking if the current user based on name is the same as the creator of a course
            // This can be used to make the course details content differ depending on ownership
            var isOwner = course.Owner.Id.ToString() == User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var users = await courseService.GetAllUsersFromCourseByIdAsync(course.Id);
            var groups = await groupService.GetAllGroupsByCourse(course.Id);
            
            var qrCode = JoinCodeUtils.GenerateJoinCodeSvg($"{Request.Scheme}://{Request.Host}{Request.PathBase}/courses/join?joinCode={course.JoinCode}");
            
            var chatMessages = await chatMessageService.GetAllChatMessagesInChannel($"course:{course.JoinCode}");
            
            var detailCourseViewModel = new CourseDetailViewModel(course, isOwner, users,groups, qrCode, chatMessages);
        
            return View(detailCourseViewModel);
        }
        catch(ArgumentException)
        {
            _logger.LogError("No course with course code: {code} was found",code);
            return RedirectToAction("Error","Home",new {code = 404,description="A Course with that code was not found"});
        }
    }
    
    [HttpGet("/courses/join")]
    public async Task<IActionResult> Join([FromQuery] string joinCode)
    {
        try
        {
            var course = await courseService.GetCourseByJoinCodeAsync(joinCode);
            // Checking if the current user based on name is the same as the creator of a course
            var isOwner = course.Owner.Id.ToString() == User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if (isOwner) return RedirectToAction("Error", "Home",new {code=500,description="You are the owner of this course"});
            await courseService.AddUserToCourseAsync(course.DisplayName, User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            return RedirectToAction("Details","Course",new {courseName=course.DisplayName});
        }
        catch(ArgumentException)
        {
            _logger.LogError("No course with join code: {joinCode} was found",joinCode);
            return RedirectToAction("Error","Home",new {code = 404,description="Could not find course with join code"});
        }
    }
    
    
    [HttpPost("/courses/create")]
    public async Task<IActionResult> CreateCourse(CourseCreationDto dto)
    {
        try
        {
            var uId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var creator = await userService.GetUserByIdAsync(uId);

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