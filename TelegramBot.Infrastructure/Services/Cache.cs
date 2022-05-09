using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TelegramBot.Infrastructure.Database.Entities;
using TelegramBot.Infrastructure.Enums;

namespace TelegramBot.Infrastructure.Services
{
    public class Cache<TEntity> where TEntity : Entity
    {
        public Dictionary<TEntity, DbEntityState> RecordsWithState { get; set; }
        public IEnumerable<TEntity> Records => RecordsWithState.Select(r => r.Key);
        private int _writeOperationsCount;
        public bool IsFull => _writeOperationsCount >= 100;

        public Cache()
        {
            _writeOperationsCount = 0;
        }

        public void AddOrUpdate(TEntity entity, Expression<Func<TEntity, bool>> findFunction = null)
        {
            TEntity existingRecord = null;
            if (findFunction != null)
            {
                existingRecord = Records.FirstOrDefault(findFunction.Compile());
                if (existingRecord != null)
                {
                    existingRecord.CopyFrom(entity);
                    if (RecordsWithState[existingRecord] == DbEntityState.Exists)
                        RecordsWithState[existingRecord] = DbEntityState.Modified;
                }
            }
            if (existingRecord == null)
                RecordsWithState.Add(entity, DbEntityState.New);
            _writeOperationsCount++;
        }

        public void Clear()
        {
            _writeOperationsCount = 0;
            RecordsWithState.Clear();
        }
    }
}
