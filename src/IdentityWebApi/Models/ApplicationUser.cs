using Microsoft.AspNetCore.Identity;

namespace IdentityWebApi.Models;

public class ApplicationUser : IdentityUser
{
    private ApplicationUser() { }

    public ApplicationUser(string name, string username, string email)
    {
        Name = name;
        UserName = username;
        Email = email;
    }

    public string Name { get; set; }
}