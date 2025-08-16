using Spendly.Domain.Interfaces;

namespace TechChallenge.Data;

public sealed class UnitOfWork(TechChallengeContext context) : IUnitOfWork
{
    public async Task<bool> CommitAsync(CancellationToken cancellationToken)
        => await context.SaveChangesAsync(cancellationToken) > 0;
}