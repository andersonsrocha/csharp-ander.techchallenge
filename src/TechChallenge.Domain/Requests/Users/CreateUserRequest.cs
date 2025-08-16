using TechChallenge.Domain.Models;

namespace TechChallenge.Domain.Requests.Users;

public class CreateUserRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    
    public static implicit operator User(CreateUserRequest request)
    {
        return new User(request.Name, request.Email);
    }
}