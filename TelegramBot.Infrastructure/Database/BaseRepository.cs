using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelegramBot.Infrastructure.Database.Entities;
using TelegramBot.Infrastructure.Database.Interfaces;

namespace TelegramBot.Infrastructure.Database
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly Func<BotContext> _contextFactory;
        protected DbSet<TEntity> DbEntitySet;

        public BaseRepository(Func<BotContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        private BotContext RefreshContext()
        {
            var context = _contextFactory();
            DbEntitySet = context.Set<TEntity>();
            return context;
        }

        public virtual List<TEntity> GetAll()
        {
            using (var context = RefreshContext())
            {
                return DbEntitySet.ToList();
            }
        }
        public void AddRange(IEnumerable<TEntity> entities)
        {
            using (var context = RefreshContext())
            {
                DbEntitySet.AddRange(entities);
                context.Commit();
            }
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = RefreshContext())
            {
                return DbEntitySet.Where(predicate).ToList();
            }
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = RefreshContext())
            {
                return Queryable.FirstOrDefault(DbEntitySet, predicate);
            }
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = RefreshContext())
            {
                return Queryable.SingleOrDefault(DbEntitySet, predicate);
            }
        }
        public bool Any(Expression<Func<TEntity, bool>> predicate = null)
        {
            using (var context = RefreshContext())
            {
                if (predicate == null)
                    return DbEntitySet.Any();
                return DbEntitySet.Any(predicate);
            }
        }

        public async Task AddOrUpdateAsync(TEntity entity, 
            Expression<Func<TEntity, bool>> findFunction = null)
        {
            TEntity existingEntity = null;
            if (findFunction != null)
                existingEntity = SingleOrDefault(findFunction);
            using (var context = RefreshContext())
            {
                if (existingEntity != null)
                {
                    existingEntity.CopyFrom(entity);
                    DbEntitySet.Attach(existingEntity);
                    context.Entry(existingEntity).State = EntityState.Modified;
                }
                else
                {
                    await DbEntitySet.AddAsync(entity).ConfigureAwait(false);
                }
                await context.CommitAsync().ConfigureAwait(false);
            }
        }

        public void Delete(TEntity entity)
        {
            using (var context = RefreshContext())
            {
                if (context.Entry(entity).State == EntityState.Detached)
                {
                    DbEntitySet.Attach(entity);
                }
                DbEntitySet.Remove(entity);
                context.Commit();
            }
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            using (var context = RefreshContext())
            {
                context.UpdateRange(entities);
                context.SaveChanges();
            }
        }
    }
}
