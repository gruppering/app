using app.DTO.Course;
using app.Models;
using app.Services;
using NUnit.Framework;

namespace app.Tests.Groups;

public class GroupIntegrationTests
{
    private readonly UserService _userService;
    private readonly GroupService _groupService;
    private readonly CourseService _courseService;
    
    private GrupperingUser? _testUser;
    private GrupperingUser? _secondTestUser;

    
    public GroupIntegrationTests()
    {
        var factory = new GrupperingTestApplicationFactory();
        var scope = factory.Services.CreateScope();
        _userService = scope.ServiceProvider.GetRequiredService<UserService>();
        _groupService = scope.ServiceProvider.GetRequiredService<GroupService>();
        _courseService = scope.ServiceProvider.GetRequiredService<CourseService>();
    }
    
    [OneTimeSetUp] 
    public async Task Setup()
    {
        _testUser = await _userService.Signup("test","test@kvok.no","test");
        _secondTestUser = await _userService.Signup("test2","test2@kvok.no","test");
    }

    [Test]
    public async Task EnsureCreatingTwoGroupInDifferingCoursesHaveSameId()
    {
        var eCourse = await _courseService.CreateNewCourse(new CourseCreationDto("testExclusiveCourse1", "...", true), _testUser!);
        var eCourse2 = await _courseService.CreateNewCourse(new CourseCreationDto("testExclusiveCourse2", "...", true), _testUser!);

        var group1FirstCourse = await _groupService.CreateNewGroupAsync("testGroup",eCourse);
        var group2SecondCourse = await _groupService.CreateNewGroupAsync("testGroup",eCourse2);
        Assert.That(group1FirstCourse.Id,Is.EqualTo(group2SecondCourse.Id));
    }
    
    [Test]
    public async Task EnsureCorrectUserMembershipIsReturned()
    {
        var eCourse = await _courseService.CreateNewCourse(new CourseCreationDto("testMembershipCourse1", "...", true), _testUser!);

        var testMembershipGroup = await _groupService.CreateNewGroupAsync("testMembershipGroup",eCourse);
        await _groupService.AddUserToGroup(_secondTestUser!, testMembershipGroup.Id, eCourse);
        var groupsMemberIn = await _groupService.GetUserGroups(_secondTestUser!.Username);
        Assert.That(groupsMemberIn.Exists(g => g.Members.Contains(_secondTestUser!)),Is.EqualTo(true));
    }
}