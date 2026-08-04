using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Application.DTOs;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Infrastructure.Data;

namespace AssignmentManagement.Infrastructure.Services;

public class TeacherAssignmentService
{
    private readonly AppDbContext _db;

    public TeacherAssignmentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<TeacherAssignmentDto>> GetAllAsync()
    {
        return await _db.TeacherAssignments
            .Include(t => t.Teacher)
            .Include(t => t.Subject)
            .Include(t => t.Class)
            .Select(t => new TeacherAssignmentDto
            {
                Id = t.Id,
                TeacherId = t.TeacherId,
                TeacherName = t.Teacher.FullName,
                SubjectId = t.SubjectId,
                SubjectName = t.Subject.Name,
                ClassId = t.ClassId,
                ClassName = t.Class.Name + " " + t.Class.Section
            })
            .ToListAsync();
    }

    public async Task<TeacherAssignmentDto?> CreateAsync(CreateTeacherAssignmentDto dto)
    {
        var teacher = await _db.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == dto.TeacherId && u.Role.Name == "Teacher");
        if (teacher == null) return null;

        var subject = await _db.Subjects.FindAsync(dto.SubjectId);
        var cls = await _db.Classes.FindAsync(dto.ClassId);
        if (subject == null || cls == null) return null;

        var entity = new TeacherAssignment
        {
            TeacherId = dto.TeacherId,
            SubjectId = dto.SubjectId,
            ClassId = dto.ClassId
        };

        _db.TeacherAssignments.Add(entity);
        await _db.SaveChangesAsync();

        return new TeacherAssignmentDto
        {
            Id = entity.Id,
            TeacherId = teacher.Id,
            TeacherName = teacher.FullName,
            SubjectId = subject.Id,
            SubjectName = subject.Name,
            ClassId = cls.Id,
            ClassName = cls.Name + " " + cls.Section
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.TeacherAssignments.FindAsync(id);
        if (entity == null) return false;
        _db.TeacherAssignments.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}
