using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentManagement.Application.DTOs;

public class SubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateSubjectDto
{
    public string Name { get; set; } = string.Empty;
}
