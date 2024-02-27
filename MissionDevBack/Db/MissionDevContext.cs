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

        builder.Entity<Project>(entity =>
            {
                entity.HasMany(p => p.ProjectUsers)
                .WithOne(pu => pu.Project)
                .HasForeignKey(pu => pu.ProjectId)
                .HasPrincipalKey(p => p.Id);

                entity.HasMany(p => p.ProjectFiles)
                .WithOne(pf => pf.Project)
                .HasForeignKey(pf => pf.ProjectId)
                .HasPrincipalKey(p => p.Id);

                entity.HasMany(p => p.Sketches)
                .WithOne(s => s.Project)
                .HasForeignKey(s => s.ProjectId)
                .HasPrincipalKey(p => p.Id);
            });

        builder.Entity<ProjectFile>(entity =>
            {
                entity.HasOne(pf => pf.Project)
                .WithMany(p => p.ProjectFiles)
                .HasForeignKey(pj => pj.ProjectId);

                entity.HasOne(pf => pf.User)
                .WithMany(u => u.ProjectFiles)
                .HasForeignKey(pj => pj.UserId);
            });

        builder.Entity<ProjectUser>(entity =>
        {
            entity.HasOne(pu => pu.Project)
            .WithMany(p => p.ProjectUsers)
            .HasForeignKey(pu => pu.ProjectId);

            entity.HasOne(pu => pu.User)
            .WithMany(u => u.ProjectUsers)
            .HasForeignKey(pu => pu.UserId);
        });

        builder.Entity<Sketch>(entity =>
            {
                entity.HasOne(s => s.Project)
                .WithMany(p => p.Sketches)
                .HasForeignKey(s => s.ProjectId);

                entity.HasOne(s => s.Author)
                .WithMany(u => u.Sketches)
                .HasForeignKey(s => s.AuthorId);
            });

        builder.Entity<User>(entity =>
            {
                entity.HasMany(u => u.ProjectUsers)
                .WithOne(pu => pu.User)
                .HasForeignKey(pj => pj.UserId)
                .HasPrincipalKey(p => p.Id);

                entity.HasMany(u => u.Sketches)
                .WithOne(s => s.Author)
                .HasForeignKey(s => s.AuthorId)
                .HasPrincipalKey(u => u.Id);

                entity.HasMany(u => u.ProjectFiles)
                .WithOne(pf => pf.User)
                .HasForeignKey(pf => pf.UserId)
                .HasPrincipalKey(u => u.Id);

                entity.HasMany(u => u.UserFiles)
                .WithOne(uf => uf.User)
                .HasForeignKey(uf => uf.UserId)
                .HasPrincipalKey(u => u.Id);
            });

        builder.Entity<UserFile>(entity =>
            {
                entity.HasOne(uf => uf.User)
                .WithMany(u => u.UserFiles)
                .HasForeignKey(uf => uf.UserId);
            });

    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectFile> ProjectFiles { get; set; }
    public DbSet<UserFile> UserFiles { get; set; }
    public DbSet<ProjectUser> ProjectUsers { get; set; }
    public DbSet<Sketch> Sketches { get; set; }
}
