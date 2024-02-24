namespace MissionDevBack.Db;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MissionDevBack.Models;

public sealed class MissionDevContext : IdentityDbContext<User>
{
    public MissionDevContext(DbContextOptions<MissionDevContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectFile> ProjectFiles { get; set; }
    public DbSet<UserFile> UserFiles { get; set; }
    public DbSet<ProjectUser> ProjectUsers { get; set; }
    public DbSet<Sketch> Sketches { get; set; }
}
