using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public class FileStorageEditFileResponse
{
    [Required]
    public bool IsSuccess { get; set; } = false;

    [Required]
    public List<string> Errors { get; set; } = [];

    public string RelativePathFromStorage { get; set; }

    public int ResourceId { get; set; }
}
