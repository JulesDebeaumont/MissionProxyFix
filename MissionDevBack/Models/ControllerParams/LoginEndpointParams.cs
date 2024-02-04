using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models.ControllerParams;

public class LoginEndpointParams {
    [Required]
    public string username { get; set;}

    [Required]
    public string password { get; set;}
}
