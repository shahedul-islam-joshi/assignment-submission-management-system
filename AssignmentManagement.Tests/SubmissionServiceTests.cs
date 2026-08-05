using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Xunit;
using AssignmentManagement.Application.DTOs;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Infrastructure.Data;
using AssignmentManagement.Infrastructure.Services;

namespace AssignmentManagement.Tests;

public class SubmissionServiceTests
{
    private AppDbContext GetDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private async Task<(AppDbContext db, Assignment assignment, User student)> SeedAsync(DateTime dueDate, string status = "Published")
    {
        var db = GetDb();

        var role = new Role { Name = "Student" };
        db.Roles.Add(role);
        await db.SaveChangesAsync();

        var cls = new Class { Name = "Grade 10", Section = "A" };
        var subject = new Subject { Name = "Math" };
        db.Classes.Add(cls);
        db.Subjects.Add(subject);
        await db.SaveChangesAsync();

        var teacherRole = new Role { Name = "Teacher" };
        db.Roles.Add(teacherRole);
        await db.SaveChangesAsync();

        var teacher = new User { FullName = "T", Email = "t@t.com", PasswordHash = "x", RoleId = teacherRole.Id };
        db.Users.Add(teacher);
        await db.SaveChangesAsync();

        var student = new User { FullName = "S", Email = "s@s.com", PasswordHash = "x", RoleId = role.Id, ClassId = cls.Id };
        db.Users.Add(student);
        await db.SaveChangesAsync();

        var assignment = new Assignment
        {
            Title = "HW1",
            Description = "desc",
            DueDate = dueDate,
            MaxMarks = 100,
            Status = status,
            TeacherId = teacher.Id,
            ClassId = cls.Id,
            SubjectId = subject.Id
        };
        db.Assignments.Add(assignment);
        await db.SaveChangesAsync();

        return (db, assignment, student);
    }

    [Fact]
    public async Task Submit_Fails_When_Deadline_Passed()
    {
        var (db, assignment, student) = await SeedAsync(DateTime.UtcNow.AddDays(-1));
        var service = new SubmissionService(db);

        var (success, error, data) = await service.SubmitAsync(student.Id, new CreateSubmissionDto
        {
            AssignmentId = assignment.Id,
            AnswerText = "my answer"
        });

        Assert.False(success);
        Assert.Contains("Deadline", error);
    }

    [Fact]
    public async Task Submit_Succeeds_Before_Deadline()
    {
        var (db, assignment, student) = await SeedAsync(DateTime.UtcNow.AddDays(1));
        var service = new SubmissionService(db);

        var (success, error, data) = await service.SubmitAsync(student.Id, new CreateSubmissionDto
        {
            AssignmentId = assignment.Id,
            AnswerText = "my answer"
        });

        Assert.True(success);
        Assert.NotNull(data);
        Assert.Equal("my answer", data!.AnswerText);
    }

    [Fact]
    public async Task Grade_Fails_When_Marks_Exceed_MaxMarks()
    {
        var (db, assignment, student) = await SeedAsync(DateTime.UtcNow.AddDays(1));
        var service = new SubmissionService(db);

        await service.SubmitAsync(student.Id, new CreateSubmissionDto { AssignmentId = assignment.Id, AnswerText = "ans" });
        var submission = db.Submissions.First();

        var (success, error) = await service.GradeAsync(assignment.TeacherId, submission.Id, new GradeSubmissionDto
        {
            Marks = 150,
            Feedback = "too high"
        });

        Assert.False(success);
        Assert.Contains("between 0 and", error);
    }

    [Fact]
    public async Task Grade_Fails_When_Not_Own_Assignment()
    {
        var (db, assignment, student) = await SeedAsync(DateTime.UtcNow.AddDays(1));
        var service = new SubmissionService(db);

        await service.SubmitAsync(student.Id, new CreateSubmissionDto { AssignmentId = assignment.Id, AnswerText = "ans" });
        var submission = db.Submissions.First();

        var wrongTeacherId = 9999;
        var (success, error) = await service.GradeAsync(wrongTeacherId, submission.Id, new GradeSubmissionDto
        {
            Marks = 80,
            Feedback = "good"
        });

        Assert.False(success);
        Assert.Contains("Not your assignment", error);
    }

    [Fact]
    public async Task One_Submission_Per_Student_Per_Assignment_Updates_Existing()
    {
        var (db, assignment, student) = await SeedAsync(DateTime.UtcNow.AddDays(1));
        var service = new SubmissionService(db);

        await service.SubmitAsync(student.Id, new CreateSubmissionDto { AssignmentId = assignment.Id, AnswerText = "first" });
        await service.SubmitAsync(student.Id, new CreateSubmissionDto { AssignmentId = assignment.Id, AnswerText = "second" });

        var count = db.Submissions.Count(s => s.AssignmentId == assignment.Id && s.StudentId == student.Id);
        Assert.Equal(1, count);
        Assert.Equal("second", db.Submissions.First().AnswerText);
    }
}
