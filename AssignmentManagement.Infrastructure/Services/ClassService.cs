using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Application.DTOs;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Infrastructure.Data;

namespace AssignmentManagement.Infrastructure.Services;

public class ClassService
{
    private readonly AppDbContext _db;

    public ClassService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ClassDto>> GetAllAsync()
    {
        return await _db.Classes
            .Select(c => new ClassDto { Id = c.Id, Name = c.Name, Section = c.Section })
            .ToListAsync();
    }

    public async Task<ClassDto> CreateAsync(CreateClassDto dto)
    {
        var entity = new Class { Name = dto.Name, Section = dto.Section };
        _db.Classes.Add(entity);
        await _db.SaveChangesAsync();
        return new ClassDto { Id = entity.Id, Name = entity.Name, Section = entity.Section };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Classes.FindAsync(id);
        if (entity == null) return false;
        _db.Classes.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}
