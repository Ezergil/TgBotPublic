using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TelegramBot.Infrastructure.Database.Entities;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.Enums;

namespace TelegramBot.Infrastructure.Services
{
    public class CachedRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly IRepository<TEntity> _repository;
        private readonly Cache<TEntity> _cache;

        public CachedRepository(IRepository<TEntity> repository,
            Cache<TEntity> cache)
        {
            _repository = repository;
            _cache = cache;
            InitializeCache();
        }
        private void InitializeCache()
        {
            _cache.RecordsWithState = _repository.GetAll().
                ToDictionary(e => e, e => DbEntityState.Exists);
        }
        public void ProcessChangesFromCache()
        {
            var entitiesToAdd = _cache.RecordsWithState.Where(r => r.Value == DbEntityState.New).
                Select(r => r.Key);
            _repository.AddRange(entitiesToAdd);
            var entitiesToModify = _cache.RecordsWithState.Where(r => r.Value == DbEntityState.Modified).
                Select(r => r.Key);
            _repository.UpdateRange(entitiesToModify);
            _cache.Clear();
            InitializeCache();
        }
        public void AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _cache.AddOrUpdate(entity);
                if (_cache.IsFull)
                {
                    ProcessChangesFromCache();
                }
            }
        }
        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _cache.Records.Where(predicate.Compile());
        }
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _cache.Records.FirstOrDefault(predicate.Compile());
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _cache.Records.SingleOrDefault(predicate.Compile());
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
                return _cache.Records.Any();
            return _cache.Records.Any(predicate.Compile());
        }

        public List<TEntity> GetAll()
        {
            return _cache.Records.ToList();
        }
        public Task AddOrUpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> findFunction = null)
        {
            _cache.AddOrUpdate(entity, findFunction);
            if (_cache.IsFull)
            {
                ProcessChangesFromCache();
            }
            return Task.CompletedTask;
        }
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }
    }
}
