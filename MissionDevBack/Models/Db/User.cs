using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MissionDevBack.Models;

public enum EUserRoles
{
    Admin,
    Worker
}

public class User : IdentityUser
{
    [Required]
    public string IdRes { get; set; }
    public string Fullname { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
    public List<EUserRoles> Roles { get; set; } = [];
    public ICollection<ProjectUser> ProjectUsers { get; set; }
}