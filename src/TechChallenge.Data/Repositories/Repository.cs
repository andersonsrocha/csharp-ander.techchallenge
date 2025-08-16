using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Models;

namespace TechChallenge.Data.Repositories;

public abstract class Repository<TEntity>(TechChallengeContext context) : IRepository<TEntity> where TEntity : Entity
{
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    
    public TEntity? Find(Guid id)
        => _dbSet.AsNoTracking().SingleOrDefault(x => x.Id == id);

    public IEnumerable<TEntity> Find()
        => _dbSet.AsNoTracking().ToList();

    public void Add(TEntity entity)
        => _dbSet.Add(entity);

    public void Update(TEntity entity)
        => _dbSet.Update(entity);

    public void Delete(TEntity entity)
        => _dbSet.Remove(entity);
}