using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public class ProjectUser
{
    public int Id { get; set;}
    
    [Required]
    public string UserId { get; set; }

    public User User { get; set; }

    [Required]
    public int ProjectId { get; set; }

    public Project Project { get; set; }

    public string MailFolderName { get; set; }
}