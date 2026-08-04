using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentManagement.Application.DTOs;

public class SubmissionDto
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AnswerText { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public int? Marks { get; set; }
    public string? Feedback { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreateSubmissionDto
{
    public int AssignmentId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
}

public class GradeSubmissionDto
{
    public int Marks { get; set; }
    public string Feedback { get; set; } = string.Empty;
}