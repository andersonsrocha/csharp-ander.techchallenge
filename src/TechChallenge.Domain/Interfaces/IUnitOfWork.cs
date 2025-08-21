﻿namespace TechChallenge.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<bool> CommitAsync(CancellationToken cancellationToken);
}