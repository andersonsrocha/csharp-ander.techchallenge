using Microsoft.EntityFrameworkCore;
using TechChallengeUsers.Domain.Interfaces;
using TechChallengeUsers.Domain.Models;

namespace TechChallengeUsers.Data.Repositories;

public class UserRepository(TechChallengeUsersContext usersContext) : IUserRepository
{
    private readonly DbSet<User> _dbSet = usersContext.Set<User>();
    
    public async Task<User?> Find(Guid id)
        => await _dbSet.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<User>> Find()
        => await _dbSet.AsNoTracking().ToListAsync();
}