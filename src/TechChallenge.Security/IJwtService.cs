using TechChallenge.Domain.Models;

namespace TechChallenge.Security;

public interface IJwtService
{
    public string Generate(User user);
}