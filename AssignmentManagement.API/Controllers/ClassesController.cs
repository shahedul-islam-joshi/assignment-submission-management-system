using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignmentManagement.Application.DTOs;
using AssignmentManagement.Infrastructure.Services;

namespace AssignmentManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ClassesController : ControllerBase
{
    private readonly ClassService _classService;

    public ClassesController(ClassService classService)
    {
        _classService = classService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _classService.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClassDto dto)
    {
        return Ok(await _classService.CreateAsync(dto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _classService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
