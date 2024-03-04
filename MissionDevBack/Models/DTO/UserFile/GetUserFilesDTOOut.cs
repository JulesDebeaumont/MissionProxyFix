namespace MissionDevBack.Models;

public class GetUserFilesDTOOut
{
    public int Id { get; set; }

    public string Filename { get; set; }

    public string MimeType { get; set; }

    public DateTime CreatedAt { get; set; }
}