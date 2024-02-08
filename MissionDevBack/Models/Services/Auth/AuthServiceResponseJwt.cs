using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public class AuthServiceResponseJwt
{
    [Required]
    public string EncodedJwtToken { get; set; }

    [Required]
    public string RefreshToken { get; set; }
}