using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentManagement.Application.DTOs;

public class RegisterDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Admin, Teacher, Student
    public int? ClassId { get; set; } // only for Student
}
