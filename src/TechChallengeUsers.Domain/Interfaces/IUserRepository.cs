using TechChallengeUsers.Domain.Models;

namespace TechChallengeUsers.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> Find(Guid id);
    Task<IEnumerable<User>> Find();
}