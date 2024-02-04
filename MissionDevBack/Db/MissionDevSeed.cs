namespace MissionDevBack.Db;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MissionDevBack.Models;

public static class MissionDevSeed
{
    public static async Task RunAsync(MissionDevContext context, UserManager<User> userManager)
    {
        if (context.Users.Any())
        {
            return;
        }

        // Users
        context.Users.ExecuteDelete();
        var users = new User[]
           {
                new() {Fullname="Jules Debeaumont", UserName="didier", Roles=[EnumRoles.Admin]},
                new() {Fullname="Brian Bertili", UserName="didierbis", Roles=[EnumRoles.Worker]}
           };
        foreach (User user in users) {
            var password = "_SuperDidier1234_";
            await userManager.CreateAsync(user, password);
        }

        // Projects
        context.Projects.ExecuteDelete();
        var projects = new Project[]
           {
                new() {Title="Flammèche"},
                new() {Title="Site de l'IIAS"},
                new() {Title="Pascomacte"},
                new() {Title="Astre"},
           };
        await context.Projects.AddRangeAsync(projects);

        context.SaveChanges();
    }
}
