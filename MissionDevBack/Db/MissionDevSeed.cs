namespace MissionDevBack.Db;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MissionDevBack.Models;

public static class MissionDevSeed
{
    public static async Task RunAsync(MissionDevContext context, UserManager<User> userManager)
    {
        if (true)
        {
            return;
        }

        // Users
        context.Users.ExecuteDelete();
        var user1 = new User() { 
            Fullname = "Jules Debeaumont",
            UserName = "JulesD", 
            IdRes = "", 
            Roles = [EUserRoles.Admin, EUserRoles.Worker], 
            Email = "jdebeaumont@chu-reims.fr"
        };
        var user2 = new User() { 
            Fullname = "Brian Bertili", 
            UserName = "BrianB", 
            IdRes = "", 
            Roles = [EUserRoles.Worker] 
        };
        var users = new User[]
           {
                user1,
                user2
           };
        foreach (User user in users)
        {
            var password = "_SuperDidier1234_";
            await userManager.CreateAsync(user, password);
        }

        // Projects
        context.Projects.ExecuteDelete();
        var project1 = new Project() { Title = "Astre", Deadline = new DateTime(2023, 8, 1), State = EProjectState.Done };
        var projects = new Project[]
           {
                new() {Title="Site de l'IIAS", Deadline= new DateTime(2023, 5, 1), State = EProjectState.Done },
                new() {Title="Flammèche", Deadline= new DateTime(2024, 6, 1), State = EProjectState.Started },
                new() {Title="Pascomacte", State = EProjectState.Pending },
                new() {Title="Intranet", Deadline= new DateTime(2023, 2, 22), State = EProjectState.Started },
           };
        await context.Projects.AddRangeAsync(project1);
        await context.Projects.AddRangeAsync(projects);

        context.SaveChanges();

        // ProjectUser
        context.ProjectUsers.ExecuteDelete();
        var projectUsers = new ProjectUser[]
        {
            new() {ProjectId=project1.Id, UserId=user1.Id}
        };

        await context.ProjectUsers.AddRangeAsync(projectUsers);

        context.SaveChanges();
    }
}
