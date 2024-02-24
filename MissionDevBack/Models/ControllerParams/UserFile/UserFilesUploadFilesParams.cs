using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public class UserFilesUploadFilesParams
{
    [Required]
    public string UserId { get; set;}
    public List<IFormFile> Files { get; set; }
}