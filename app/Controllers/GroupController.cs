using app.DTO.Group;
using app.Models;
using app.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers;

[Authorize]
[ApiController]
public class GroupController(GroupService groupService,CourseService courseService): ControllerBase
{

    [HttpPost("/course/{courseName}/groups/new")]
    public async Task<List<Group>> GenerateNewGroups(string courseName,[FromBody] GroupsCreationDto creationDto)
    {
        var course = await courseService.GetCourseByNameAsync(courseName);
        return await groupService.DelegateUsersToGroups(course.Id, true, creationDto.groupAmount,creationDto.groupPrefix);
    }
}