using app.DTO.Course;
using app.Models;
using app.Services;
using NUnit.Framework;

namespace app.Tests.Courses;

// Compiler is stupid, and doesn't understand that the OneTimeSetup is run before the others, actually creating the users, etc.
public class CourseIntegrationTests
{
    private readonly GrupperingTestApplicationFactory _factory = new();
    private CourseService _courseService;
    private UserService _userService;

    private IServiceScope? _scope;
    private GrupperingUser? _testUser;
    private GrupperingUser? _secondTestUser;
    private HashSet<string> _createdCourseNames = [];
    
    [OneTimeSetUp] 
    public async Task Setup()
    {
        _scope = _factory.Services.CreateScope();
        _courseService = _scope.ServiceProvider.GetRequiredService<CourseService>();
        _userService = _scope.ServiceProvider.GetRequiredService<UserService>();
        _testUser = await _userService.Signup("test","test@kvok.no","test");
        _secondTestUser = await _userService.Signup("test2","test2@kvok.no","test");

    }


    [Test]
    public async Task EnsureCreatingCourseCompletesSuccessfully()
    {
        var courseDto = new CourseCreationDto("test", "...", true);
        _createdCourseNames.Add("test");
        var createdCourse = await _courseService.CreateNewCourse(courseDto, _testUser!);
        Assert.That(createdCourse.DisplayName,Is.EqualTo("test"));
    }
    
    [Test]
    public async Task EnsureUserGetsAddedToCourseCorrectly()
    {
        var courseDto = new CourseCreationDto("testAddition", "...", true);
        _createdCourseNames.Add("testAddition");
        await _courseService.CreateNewCourse(courseDto, _testUser!);
        await _courseService.AddUserToCourse("testAddition", _secondTestUser!);
        var updatedCourse = await _courseService.GetCourseByNameAsync("testAddition");
        Assert.That(updatedCourse.Members.Contains(_secondTestUser!),Is.EqualTo(true));
    }
    
    [Test]
    public async Task EnsureUserGetsRemovedFromCourseCorrectly()
    {
        var courseDto = new CourseCreationDto("testAddition2", "...", true);
        _createdCourseNames.Add("testAddition2");
        await _courseService.CreateNewCourse(courseDto, _testUser!);
        await _courseService.AddUserToCourse("testAddition2", _secondTestUser!);
        await _courseService.RemoveUserFromCourse("testAddition2", _secondTestUser!);
        var updatedCourse = await _courseService.GetCourseByNameAsync("testAddition2");
        Assert.That(updatedCourse.Members.Contains(_secondTestUser!),Is.EqualTo(false));
    }
    
    [Test]
    public async Task EnsureRemovingCourseCompletesSuccessfully()
    {
        var courseDto = new CourseCreationDto("testAddition3", "...", true);
        await _courseService.CreateNewCourse(courseDto, _testUser!);
        await _courseService.DeleteCourse(courseDto.CourseName, _testUser!);
        Assert.ThrowsAsync<ArgumentException>(async () => await _courseService.GetCourseByNameAsync("testAddition3"));
    }

    
    
    [OneTimeTearDown]
    public async Task Completion()
    {
        foreach (var courseName in _createdCourseNames)
        {
            await _courseService.DeleteCourse(courseName, _testUser!);
        }
    }
}