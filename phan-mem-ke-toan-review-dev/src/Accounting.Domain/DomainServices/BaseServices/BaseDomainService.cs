using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Accounting.DomainServices.BaseServices
{
    public class BaseDomainService<TEntity,TKey> : DomainService where TEntity : class, IEntity<TKey>
    {
        #region Fields
        private readonly IRepository<TEntity, TKey> _repository;
        #endregion
        #region Ctor
        public BaseDomainService(IRepository<TEntity, TKey> repository)
        {
            _repository = repository;
        }
        #endregion
        #region Methods
        public virtual async Task<TEntity> CreateAsync(TEntity entity,bool autoSave=false)
        {
            await CheckDuplicate(entity);
            return await _repository.InsertAsync(entity, autoSave);
        }
        public virtual async Task CreateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            await _repository.InsertManyAsync(entities, autoSave);
        }    
        public virtual async Task UpdateAsync(TEntity entity,bool autoSave = false)
        {
            await CheckDuplicate(entity);
            await _repository.UpdateAsync(entity, autoSave);
        }
        public virtual async Task UpdateNoCheckDuplicateAsync(TEntity entity, bool autoSave = false)
        {
            await _repository.UpdateAsync(entity, autoSave);
        }
        public virtual async Task DeleteAsync(TEntity entity, bool autoSave = false)
        {
            await _repository.DeleteAsync(entity, autoSave);
        }
        public virtual async Task DeleteAsync(TKey id, bool autoSave = false)
        {
            await _repository.DeleteAsync(id, autoSave);
        }
        public virtual async Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false)
        {
            if (ids == null) return;
            await _repository.DeleteManyAsync(ids, autoSave);
        }
        public virtual async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            if (entities == null) return;
            await _repository.DeleteManyAsync(entities, autoSave);
        }
        public virtual async Task<TEntity> GetAsync(TKey id)
        {
            return await _repository.GetAsync(id);
        }
        public virtual async Task<TEntity> FindAsync(TKey id)
        {
            return await _repository.FindAsync(id);
        }
        public virtual async Task<IQueryable<TEntity>> GetQueryableAsync()
        {
            return await _repository.GetQueryableAsync();
        }
        public virtual Task CheckDuplicate(TEntity entity)
        {
            return Task.CompletedTask;
        }
        public virtual IRepository<TEntity, TKey> GetRepository()
        {
            return _repository;
        }      
        #endregion
    }
}
