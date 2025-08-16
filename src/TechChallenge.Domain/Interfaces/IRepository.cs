﻿using TechChallenge.Domain.Models;

namespace TechChallenge.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : Entity
{
    TEntity? Find(Guid id);
    IEnumerable<TEntity> Find();
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}