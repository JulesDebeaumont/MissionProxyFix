using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public class Sketch
{
    public int Id { get; set; }

    [Required]
    public bool IsShared { get; set; } = false;

    [Required]
    public string Title { get; set; }

    [Required]
    public int AuthorId { get; set; }

    public User Author { get; set; }

    [Required]
    public int ProjectId { get; set; }

    public Project Project { get; set; }
}