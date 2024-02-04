namespace MissionDevBack.Models;

public class AuthServiceResponse {
    public bool IsLogedIn { get; set;} = false;
    public string EncodedJwtToken { get; set; }
    public string RefreshToken { get; set; }
}