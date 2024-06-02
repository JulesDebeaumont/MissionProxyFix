namespace MissionDevBack.Models;

public class ProjectGetZimbraLastMails
{
    public int ProjectId { get; set; }
    public int MailCount { get; set; }
    public string AuthToken { get; set; }
}