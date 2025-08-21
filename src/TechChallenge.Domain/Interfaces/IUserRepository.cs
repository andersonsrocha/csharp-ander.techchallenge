using TechChallenge.Domain.Models;

namespace TechChallenge.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> Find(Guid id);
    Task<IEnumerable<User>> Find();
}