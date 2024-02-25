using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public class ProjectFile
{
    public int Id { get; set; }

    [Required]
    public string Filename { get; set; }

    [Required]
    public string StorageFilename { get; set; }

    [Required]
    public string MimeType { get; set; } = "application/octet-stream";

    [Required]
    public DateTime CreatedAt { get; set; }

    public string UserId { get; set; }

    public User User { get; set; }

    public string FromMailId { get; set; }

    [Required]
    public bool IsShared { get; set; } = false;

    [Required]
    public int ProjectId { get; set; }

    public Project Project { get; set; }
}