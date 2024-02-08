using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models.ControllerParams;

public class LoginEndpointParams {
    [Required]
    public string Username { get; set;}

    [Required]
    public string Password { get; set;}

    // [Required]
    // public string CasTicket { get; set;}

    // [Required]
    // public string Service { get; set;}
}
