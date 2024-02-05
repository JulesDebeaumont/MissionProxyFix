using Microsoft.AspNetCore.Identity;

namespace MissionDevBack.Models;

public enum EnumRoles
{
    Admin,
    Worker
}

public class User : IdentityUser
{
    public string Fullname { get; set; }

    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
    public List<EnumRoles> Roles { get; set; }
    public ICollection<Project> Projects { get; set; }
}