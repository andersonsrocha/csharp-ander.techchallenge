using TechChallengeUsers.Domain.Interfaces;

namespace TechChallengeUsers.Data;

public sealed class UnitOfWork(TechChallengeUsersContext usersContext) : IUnitOfWork
{
    public async Task<bool> CommitAsync(CancellationToken cancellationToken)
        => await usersContext.SaveChangesAsync(cancellationToken) > 0;
}