using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentManagement.Domain.Entities;

public class Class
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g. "Grade 10"
    public string Section { get; set; } = string.Empty; // e.g. "A"

    public ICollection<User> Students { get; set; } = new List<User>();
    public ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
