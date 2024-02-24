using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public class ProjectGetZimbraMailAttachment
{
    [Required]
    public int ProjectId { get; set; }

    [Required]
    public string MailId { get; set; }

    [Required]
    public string Part { get; set; }

    [Required]
    public string AuthToken { get; set; }

    [Required]
    public string Filename { get; set; }
}
