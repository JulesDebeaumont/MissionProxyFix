using System.ComponentModel.DataAnnotations;

namespace MissionDevBack.Models;

public class UserFile 
{
    public int Id { get; set; }
    
    [Required]
    public string Filename { get; set; }
    
    [Required]
    public string StorageFilename { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public string UserId { get; set; }

    public User User { get; set; }
}