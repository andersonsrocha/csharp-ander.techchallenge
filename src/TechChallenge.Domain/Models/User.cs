using Microsoft.AspNetCore.Identity;

namespace TechChallenge.Domain.Models;

public sealed class User : IdentityUser<Guid>
{
    public User() { }

    public User(string name, string userName) : base(userName)
    {
        Name = name;
        Email = userName;
        NormalizedUserName = userName.ToUpper();
        NormalizedEmail = userName.ToUpper();
    }

    public string Name { get; set; } = string.Empty;
}