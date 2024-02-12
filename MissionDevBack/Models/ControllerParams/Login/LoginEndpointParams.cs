using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models.ControllerParams;

public class LoginEndpointParams {
    [Required]
    public string Username { get; set;}

    [Required]
    public string Password { get; set;}
}
