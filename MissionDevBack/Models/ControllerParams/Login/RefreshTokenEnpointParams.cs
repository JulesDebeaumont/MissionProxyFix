using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models.ControllerParams;

public class RefreshTokenEnpointParams {
    [Required]
    public string Jwt { get; set;}

    [Required]
    public string RefreshToken { get; set; }
}