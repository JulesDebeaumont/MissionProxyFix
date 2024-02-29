using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public class UserFilesUploadFilesParams
{
    public List<IFormFile> Files { get; set; }
}