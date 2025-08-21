using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Models;

namespace TechChallenge.Data.Repositories;

public class UserRepository(TechChallengeContext context) : IUserRepository
{
    private readonly DbSet<User> _dbSet = context.Set<User>();
    
    public async Task<User?> Find(Guid id)
        => await _dbSet.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<User>> Find()
        => await _dbSet.AsNoTracking().ToListAsync();
}