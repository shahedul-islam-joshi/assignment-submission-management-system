using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Application.DTOs;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Infrastructure.Data;

namespace AssignmentManagement.Infrastructure.Services;

public class AssignmentService
{
    private readonly AppDbContext _db;

    public AssignmentService(AppDbContext db)
    {
        _db = db;
    }

    private static AssignmentDto ToDto(Assignment a) => new AssignmentDto
    {
        Id = a.Id,
        Title = a.Title,
        Description = a.Description,
        DueDate = a.DueDate,
        MaxMarks = a.MaxMarks,
        Status = a.Status,
        TeacherId = a.TeacherId,
        ClassId = a.ClassId,
        ClassName = a.Class.Name + " " + a.Class.Section,
        SubjectId = a.SubjectId,
        SubjectName = a.Subject.Name
    };

    // Teacher: only their own assignments
    public async Task<List<AssignmentDto>> GetByTeacherAsync(int teacherId)
    {
        return await _db.Assignments
            .Include(a => a.Class).Include(a => a.Subject)
            .Where(a => a.TeacherId == teacherId)
            .Select(a => ToDto(a))
            .ToListAsync();
    }

    // Student: only Published assignments for their class
    public async Task<List<AssignmentDto>> GetByClassAsync(int classId)
    {
        return await _db.Assignments
            .Include(a => a.Class).Include(a => a.Subject)
            .Where(a => a.ClassId == classId && a.Status == "Published")
            .Select(a => ToDto(a))
            .ToListAsync();
    }

    public async Task<AssignmentDto?> CreateAsync(int teacherId, CreateAssignmentDto dto)
    {
        // Ensure teacher is assigned to this subject+class
        var isAssigned = await _db.TeacherAssignments.AnyAsync(t =>
            t.TeacherId == teacherId && t.SubjectId == dto.SubjectId && t.ClassId == dto.ClassId);
        if (!isAssigned) return null;

        var entity = new Assignment
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc),
            MaxMarks = dto.MaxMarks,
            ClassId = dto.ClassId,
            SubjectId = dto.SubjectId,
            TeacherId = teacherId,
            Status = "Draft"
        };

        _db.Assignments.Add(entity);
        await _db.SaveChangesAsync();

        await _db.Entry(entity).Reference(a => a.Class).LoadAsync();
        await _db.Entry(entity).Reference(a => a.Subject).LoadAsync();
        return ToDto(entity);
    }

    public async Task<AssignmentDto?> UpdateAsync(int teacherId, int id, UpdateAssignmentDto dto)
    {
        var entity = await _db.Assignments.Include(a => a.Class).Include(a => a.Subject)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (entity == null || entity.TeacherId != teacherId) return null;

        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.DueDate = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc);
        entity.MaxMarks = dto.MaxMarks;
        await _db.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<bool> DeleteAsync(int teacherId, int id)
    {
        var entity = await _db.Assignments.FindAsync(id);
        if (entity == null || entity.TeacherId != teacherId) return false;
        _db.Assignments.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PublishAsync(int teacherId, int id)
    {
        var entity = await _db.Assignments.FindAsync(id);
        if (entity == null || entity.TeacherId != teacherId) return false;
        entity.Status = "Published";
        await _db.SaveChangesAsync();
        return true;
    }
}
