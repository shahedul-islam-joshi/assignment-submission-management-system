using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignmentManagement.Application.DTOs;
using AssignmentManagement.Infrastructure.Services;

namespace AssignmentManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly AssignmentService _service;

    public AssignmentsController(AssignmentService service)
    {
        _service = service;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("teacher")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMyAssignments()
    {
        return Ok(await _service.GetByTeacherAsync(CurrentUserId));
    }

    [HttpGet("class/{classId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetForMyClass(int classId)
    {
        return Ok(await _service.GetByClassAsync(classId));
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create(CreateAssignmentDto dto)
    {
        var result = await _service.CreateAsync(CurrentUserId, dto);
        if (result == null) return BadRequest("You are not assigned to this Subject+Class.");
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Update(int id, UpdateAssignmentDto dto)
    {
        var result = await _service.UpdateAsync(CurrentUserId, id, dto);
        if (result == null) return Forbid();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(CurrentUserId, id);
        if (!success) return Forbid();
        return NoContent();
    }

    [HttpPost("{id}/publish")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Publish(int id)
    {
        var success = await _service.PublishAsync(CurrentUserId, id);
        if (!success) return Forbid();
        return NoContent();
    }
}