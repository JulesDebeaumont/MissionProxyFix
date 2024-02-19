namespace MissionDevBack.Models;

public class ZimbraServiceResponseMailsPreview
{
    public bool IsSuccess { get; set; } = false;
    
    public List<string> Errors { get; set; } = [];

    public List<ZimbraMailPreview> MailPreviews { get; set; } = [];
}