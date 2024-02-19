namespace MissionDevBack.Models;

public class ZimbraServiceResponseMail
{
    public bool IsSuccess { get; set; } = false;
    
    public List<string> Errors { get; set; } = [];

    public ZimbraMail Mail { get; set; }
}