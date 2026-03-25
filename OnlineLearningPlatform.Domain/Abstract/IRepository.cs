using OnlineLearningPlatform.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Domain.Abstract
{
    public interface IRepository<TEntity, in TKey> where TEntity : class, IEntity<TKey>
    {
        IQueryable<TEntity> FindAll(params Expression<Func<TEntity, object>>[] includeProperties);

        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);

        Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);

        Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Remove(TEntity entity);

        Task RemoveAsync(TKey id, CancellationToken cancellationToken = default);

        void RemoveMultiple(List<TEntity> entities);
    }
}
