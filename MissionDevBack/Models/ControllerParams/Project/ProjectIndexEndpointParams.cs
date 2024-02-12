using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public class ProjectIndexEndpointParams
{
    [Required]
    public int limit { get; set; }
    
    [Required]
    public int offset { get; set; }
}