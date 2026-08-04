using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignmentManagement.Application.DTOs;
using AssignmentManagement.Infrastructure.Services;

namespace AssignmentManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly SubmissionService _service;

    public SubmissionsController(SubmissionService service)
    {
        _service = service;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Submit(CreateSubmissionDto dto)
    {
        var (success, error, data) = await _service.SubmitAsync(CurrentUserId, dto);
        if (!success) return BadRequest(error);
        return Ok(data);
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMine()
    {
        return Ok(await _service.GetMySubmissionsAsync(CurrentUserId));
    }

    [HttpGet("assignment/{assignmentId}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetByAssignment(int assignmentId)
    {
        var (success, error, data) = await _service.GetByAssignmentAsync(CurrentUserId, assignmentId);
        if (!success) return Forbid();
        return Ok(data);
    }

    [HttpPut("{id}/grade")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Grade(int id, GradeSubmissionDto dto)
    {
        var (success, error) = await _service.GradeAsync(CurrentUserId, id, dto);
        if (!success) return BadRequest(error);
        return NoContent();
    }
}