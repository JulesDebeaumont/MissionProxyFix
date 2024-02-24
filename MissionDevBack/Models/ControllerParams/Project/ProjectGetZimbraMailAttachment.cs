namespace MissionDevBack.Models;

public class ProjectGetZimbraMailAttachment
{
    public int ProjectId { get; set; }
    public string MailId { get; set; }
    public string Part { get; set; }
    public string AuthToken { get; set; }
}
