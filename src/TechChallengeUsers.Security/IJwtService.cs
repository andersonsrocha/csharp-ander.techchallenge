using TechChallengeUsers.Domain.Models;

namespace TechChallengeUsers.Security;

public interface IJwtService
{
    public string Generate(User user, IEnumerable<string> roles);
}