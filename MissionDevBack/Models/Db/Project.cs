using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public class Project {
    public int Id { get; set;}

    [Required]
    public string Title { get; set;}
    public string Description { get; set;}
    public DateTime Deadline { get; set;} 
}