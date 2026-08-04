using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentManagement.Domain.Entities;

public class Assignment
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int MaxMarks { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Published, Closed

    public int TeacherId { get; set; }
    public User Teacher { get; set; } = null!;

    public int ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
