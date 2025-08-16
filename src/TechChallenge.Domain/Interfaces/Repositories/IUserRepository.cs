using TechChallenge.Domain.Models;

namespace TechChallenge.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> Find(Guid id);
    Task<IEnumerable<User>> Find();
}