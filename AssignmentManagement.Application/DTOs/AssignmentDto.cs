using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentManagement.Application.DTOs;

public class AssignmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int MaxMarks { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
}

public class CreateAssignmentDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int MaxMarks { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
}

public class UpdateAssignmentDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int MaxMarks { get; set; }
}