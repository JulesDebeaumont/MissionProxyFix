using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public class FileStorageGetFileResponse
{
    [Required]
    public bool IsSuccess { get; set; } = false;

    [Required]
    public List<string> Errors { get; set; } = [];

    public byte[] FileBytes { get; set; }
}