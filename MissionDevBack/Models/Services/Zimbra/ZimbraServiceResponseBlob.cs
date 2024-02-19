namespace MissionDevBack.Models;

public class ZimbraServiceResponseBlob
{
    public bool IsSuccess { get; set; } = false;
    
    public List<string> Errors { get; set; } = [];

    public byte[] Blob { get; set; }
}