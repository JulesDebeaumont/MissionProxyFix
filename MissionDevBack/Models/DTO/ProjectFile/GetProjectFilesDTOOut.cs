namespace MissionDevBack.Models;

public class GetProjectFilesDTOOut
{
    public int Id { get; set; }

    public string Filename { get; set; }

    public string MimeType { get; set; } = "application/octet-stream";

    public DateTime CreatedAt { get; set; }

    public string UserId { get; set; }

    public string FromMailId { get; set; }

    public bool IsShared { get; set; } = false;

    public int ProjectId { get; set; }

    public UserDTO User { get; set; }

    public class UserDTO
    {
        public string Id { get; set; }

        public string Fullname { get; set; }
    }
}