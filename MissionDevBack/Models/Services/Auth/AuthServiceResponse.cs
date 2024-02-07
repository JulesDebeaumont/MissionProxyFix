namespace MissionDevBack.Models;

public class AuthServiceResponse
{
    public bool IsLogedIn { get; set;} = false;
    public List<string> Errors { get; set; } = [];
    public string EncodedJwtToken { get; set; }
    public string RefreshToken { get; set; }
}