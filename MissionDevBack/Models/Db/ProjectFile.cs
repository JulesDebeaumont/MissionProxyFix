namespace MissionDevBack.Models;

public class ProjectFile 
{
    public int Id { get; set; }

    public string Filename { get; set; }

    public string StorageFilename { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public int ProjectId { get; set; }

    public Project Project { get; set; }
}