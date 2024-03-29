namespace MissionDevBack.Models;

public class ZimbraServiceResponseToken
{
    public bool IsSuccess { get; set; } = false;
    
    public List<string> Errors { get; set; } = [];

    public string Token { get; set; }
}
