using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignmentManagement.Application.DTOs;
using AssignmentManagement.Infrastructure.Services;

namespace AssignmentManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class TeacherAssignmentsController : ControllerBase
{
    private readonly TeacherAssignmentService _service;

    public TeacherAssignmentsController(TeacherAssignmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeacherAssignmentDto dto)
    {
        var result = await _service.CreateAsync(dto);
        if (result == null) return BadRequest("Invalid Teacher, Subject, or Class.");
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}