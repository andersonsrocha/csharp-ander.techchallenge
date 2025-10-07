using TechChallengeUsers.Domain.Models;

namespace TechChallengeUsers.Application.Users.Commands;

public class LoginUserRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}