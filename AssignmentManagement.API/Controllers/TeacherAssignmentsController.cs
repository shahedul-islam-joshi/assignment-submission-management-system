using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignmentManagement.Application.DTOs;
using AssignmentManagement.Infrastructure.Services;

namespace AssignmentManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeacherAssignmentsController : ControllerBase
{
    private readonly TeacherAssignmentService _service;

    public TeacherAssignmentsController(TeacherAssignmentService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMine()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(claimValue) || !int.TryParse(claimValue, out var teacherId))
        {
            return Unauthorized("Teacher ID claim is missing or invalid.");
        }

        var all = await _service.GetAllAsync();
        return Ok(all.Where(a => a.TeacherId == teacherId));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateTeacherAssignmentDto dto)
    {
        var result = await _service.CreateAsync(dto);
        if (result == null) return BadRequest("Invalid Teacher, Subject, or Class.");
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
