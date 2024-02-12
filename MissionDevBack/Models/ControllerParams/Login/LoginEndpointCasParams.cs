using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models.ControllerParams;

public class LoginEndpointCasParams {

    [Required]
    public string CasTicket { get; set;}

    [Required]
    public string Service { get; set;}
}
