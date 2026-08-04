using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Application.DTOs;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Infrastructure.Data;

namespace AssignmentManagement.Infrastructure.Services;

public class SubmissionService
{
    private readonly AppDbContext _db;

    public SubmissionService(AppDbContext db)
    {
        _db = db;
    }

    private static SubmissionDto ToDto(Submission s) => new SubmissionDto
    {
        Id = s.Id,
        AssignmentId = s.AssignmentId,
        AssignmentTitle = s.Assignment.Title,
        StudentId = s.StudentId,
        StudentName = s.Student.FullName,
        AnswerText = s.AnswerText,
        SubmittedAt = s.SubmittedAt,
        Marks = s.Marks,
        Feedback = s.Feedback,
        Status = s.Status
    };

    // Student submits or updates their submission (before deadline only)
    public async Task<(bool success, string? error, SubmissionDto? data)> SubmitAsync(int studentId, CreateSubmissionDto dto)
    {
        var assignment = await _db.Assignments.FindAsync(dto.AssignmentId);
        if (assignment == null || assignment.Status != "Published")
            return (false, "Assignment not found or not published.", null);

        if (DateTime.UtcNow > assignment.DueDate)
            return (false, "Deadline has passed.", null);

        var existing = await _db.Submissions
            .Include(s => s.Assignment).Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.AssignmentId == dto.AssignmentId && s.StudentId == studentId);

        if (existing != null)
        {
            existing.AnswerText = dto.AnswerText;
            existing.SubmittedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return (true, null, ToDto(existing));
        }

        var entity = new Submission
        {
            AssignmentId = dto.AssignmentId,
            StudentId = studentId,
            AnswerText = dto.AnswerText,
            SubmittedAt = DateTime.UtcNow,
            Status = "Submitted"
        };

        _db.Submissions.Add(entity);
        await _db.SaveChangesAsync();

        await _db.Entry(entity).Reference(s => s.Assignment).LoadAsync();
        await _db.Entry(entity).Reference(s => s.Student).LoadAsync();
        return (true, null, ToDto(entity));
    }

    // Student: view only their own submissions
    public async Task<List<SubmissionDto>> GetMySubmissionsAsync(int studentId)
    {
        return await _db.Submissions
            .Include(s => s.Assignment).Include(s => s.Student)
            .Where(s => s.StudentId == studentId)
            .Select(s => ToDto(s))
            .ToListAsync();
    }

    // Teacher: view submissions for one of their own assignments
    public async Task<(bool success, string? error, List<SubmissionDto>? data)> GetByAssignmentAsync(int teacherId, int assignmentId)
    {
        var assignment = await _db.Assignments.FindAsync(assignmentId);
        if (assignment == null || assignment.TeacherId != teacherId)
            return (false, "Not your assignment.", null);

        var list = await _db.Submissions
            .Include(s => s.Assignment).Include(s => s.Student)
            .Where(s => s.AssignmentId == assignmentId)
            .Select(s => ToDto(s))
            .ToListAsync();

        return (true, null, list);
    }

    // Teacher: grade a submission (only for their own assignment)
    public async Task<(bool success, string? error)> GradeAsync(int teacherId, int submissionId, GradeSubmissionDto dto)
    {
        var submission = await _db.Submissions.Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.Id == submissionId);
        if (submission == null) return (false, "Submission not found.");
        if (submission.Assignment.TeacherId != teacherId) return (false, "Not your assignment.");
        if (dto.Marks < 0 || dto.Marks > submission.Assignment.MaxMarks)
            return (false, $"Marks must be between 0 and {submission.Assignment.MaxMarks}.");

        submission.Marks = dto.Marks;
        submission.Feedback = dto.Feedback;
        submission.Status = "Graded";
        await _db.SaveChangesAsync();
        return (true, null);
    }
}
