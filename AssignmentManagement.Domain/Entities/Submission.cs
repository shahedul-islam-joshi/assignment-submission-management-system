using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentManagement.Domain.Entities;

public class Submission
{
    public int Id { get; set; }

    public int AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;

    public int StudentId { get; set; }
    public User Student { get; set; } = null!;

    public string AnswerText { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }

    public int? Marks { get; set; }
    public string? Feedback { get; set; }
    public string Status { get; set; } = "Submitted"; // Submitted, Graded
}
