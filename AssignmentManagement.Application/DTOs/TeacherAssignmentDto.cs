using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentManagement.Application.DTOs;

public class TeacherAssignmentDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
}

public class CreateTeacherAssignmentDto
{
    public int TeacherId { get; set; }
    public int SubjectId { get; set; }
    public int ClassId { get; set; }
}
