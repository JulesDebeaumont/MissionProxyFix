namespace MissionDevBack.Models;

public class ZimbraServiceResponseMails
{
    public bool IsSuccess { get; set; } = false;
    
    public List<string> Errors { get; set; } = [];

    public List<ZimbraMail> Mails { get; set; } = [];
}