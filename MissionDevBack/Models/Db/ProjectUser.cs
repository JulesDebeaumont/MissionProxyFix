using Microsoft.CodeAnalysis;

namespace MissionDevBack.Models;

public class ProjectUser
{
    public int Id { get; set;}
    
    public string UserId { get; set; }

    public User User { get; set; }

    public int ProjectId { get; set; }

    public Project Project { get; set; }

    public string MailFolderName { get; set; }
}