using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TelegramBot.Infrastructure.Database.Interfaces
{
    public interface IRepository<TEntity>
    {
        void AddRange(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        bool Any(Expression<Func<TEntity, bool>> predicate = null);
        List<TEntity> GetAll();
        void UpdateRange(IEnumerable<TEntity> entities);
        Task AddOrUpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> findFunction = null);
    }
}