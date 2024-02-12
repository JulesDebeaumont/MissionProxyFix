namespace MissionDevBack.Models;

public class AuthServiceResponseUser
{
    public bool IsSuccess { get; set; }

    public List<string> Errors { get; set; }

    public User User { get; set; }
}