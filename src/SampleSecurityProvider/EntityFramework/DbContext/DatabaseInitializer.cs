using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SampleSecurityProvider.Models;

namespace SampleSecurityProvider.EntityFramework.DbContext;

public class DatabaseInitializer(IServiceScopeFactory factory)
{
    public async Task InitializeAsync()
    {
        var scope = factory.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<SqliteDbContext>();

        await context.Database.MigrateAsync();
        await SeedAsync(services);
    }

    private static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        ApplicationUser[] users =
        [
            new("admin", "admin@email.com") { EmailConfirmed = true },
            new("test1", "test1@email.com") { EmailConfirmed = true }
        ];

        IdentityRole[] roles =
        [
            new("Admin"),
            new("TReports.Admin")
        ];

        foreach (var user in users) await userManager.CreateAsync(user, "test@123");
        foreach (var role in roles) await roleManager.CreateAsync(role);

        await userManager.AddToRoleAsync(users[0], roles[0].Name!);
        await userManager.AddToRoleAsync(users[1], roles[1].Name!);
    }
}