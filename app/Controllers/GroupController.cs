using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using app.DTO.Course;
using app.DTO.Group;
using app.Models;
using app.Services;
using app.ViewModel;
using Microsoft.AspNetCore.Authorization;

using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace app.Controllers;

public class GroupController(GroupService groupService,CourseService courseService): Controller
{
    [Authorize]
    [Produces("application/json")]
    [HttpPost("/course/{courseId}/group/new")]
    public async Task<List<Group>> GenerateNewGroups(Guid courseId, [FromBody] GroupsCreationDto creationDto)
    {
        var course = await courseService.GetCourseByIdAsync(courseId);
        return await groupService.DelegateUsersToGroups(course.Id, true, creationDto.groupAmount,creationDto.groupPrefix);
    }

    [HttpGet("/course/{joinCode}/group/{groupName}")]
    public async Task<IActionResult> Group(string joinCode, string groupName)
    {
        try
        {
            var group = await groupService.GetGroupByNameAndJoinCode(groupName, joinCode);
            var course = await courseService.GetCourseByJoinCodeAsync(joinCode);
            var isOwner = course.Owner.Id.ToString() == User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var model = new GroupDetailViewModel(group, course, isOwner);
            return View(model);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}