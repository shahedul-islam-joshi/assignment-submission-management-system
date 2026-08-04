using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignmentManagement.Application.DTOs;
using AssignmentManagement.Infrastructure.Services;

namespace AssignmentManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SubjectsController : ControllerBase
{
    private readonly SubjectService _subjectService;

    public SubjectsController(SubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _subjectService.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSubjectDto dto)
    {
        return Ok(await _subjectService.CreateAsync(dto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _subjectService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}