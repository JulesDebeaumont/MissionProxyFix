using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public enum EProjectState
{
    Started,
    Pending,
    Done,
    Canceled
}

public class Project
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime Deadline { get; set; }

    [Required]
    public EProjectState State { get; set; } = EProjectState.Pending;

    public ICollection<ProjectUser> ProjectUsers { get; set; }
}