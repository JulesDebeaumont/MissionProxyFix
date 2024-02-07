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

    [Required]
    public int ProjectId { get; set; }

    public Project Project { get; set; }
}