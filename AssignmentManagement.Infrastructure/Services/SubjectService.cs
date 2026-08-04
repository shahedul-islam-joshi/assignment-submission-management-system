using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Application.DTOs;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Infrastructure.Data;

namespace AssignmentManagement.Infrastructure.Services;

public class SubjectService
{
    private readonly AppDbContext _db;

    public SubjectService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<SubjectDto>> GetAllAsync()
    {
        return await _db.Subjects
            .Select(s => new SubjectDto { Id = s.Id, Name = s.Name })
            .ToListAsync();
    }

    public async Task<SubjectDto> CreateAsync(CreateSubjectDto dto)
    {
        var entity = new Subject { Name = dto.Name };
        _db.Subjects.Add(entity);
        await _db.SaveChangesAsync();
        return new SubjectDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Subjects.FindAsync(id);
        if (entity == null) return false;
        _db.Subjects.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}
