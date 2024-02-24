using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public class ProjectFilesUploadFilesParams
{
    [Required]
    public int ProjectId { get; set;}
    public List<IFormFile> files { get; set; }
}