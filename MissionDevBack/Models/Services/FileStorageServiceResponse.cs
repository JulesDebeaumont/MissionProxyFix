namespace MissionDevBack.Models;

public class FileStorageServiceResponse
{
    public bool IsSuccess { get; set; } = false;

    public List<string> Errors { get; set; }

    public byte[] FileBytes { get; set; }
}
