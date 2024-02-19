namespace MissionDevBack.Models;

public class ProjectGetZimbraLastMails
{
    public string FolderName { get; set; }
    public int MailCount { get; set; }
    public string AuthToken { get; set; }
}