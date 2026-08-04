using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentManagement.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Admin, Teacher, Student

    public ICollection<User> Users { get; set; } = new List<User>();
}
