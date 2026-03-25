using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Domain.Abstract;
using OnlineLearningPlatform.Domain.Entities.Base;
using OnlineLearningPlatform.DataAccess.Data;
using System.Linq.Expressions;

namespace OnlineLearningPlatform.DataAccess.Repository
{
    public class EfRepository<TEntity, TKey> : IRepository<TEntity, TKey>
         where TEntity : class, IEntity<TKey>
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public EfRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        #region Read Operations

        public IQueryable<TEntity> FindAll(params Expression<Func<TEntity, object>>[]? includeProperties)
        {
            IQueryable<TEntity> items = _dbSet.AsNoTracking();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }
            return items;
        }

        public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[]? includeProperties)
        {
            IQueryable<TEntity> items = _dbSet.AsNoTracking();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }
            return items.Where(predicate);
        }

        #endregion

        #region Read Operations (IO-Bound - Async)

        public async Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return await FindAll(includeProperties).SingleOrDefaultAsync(e => e.Id!.Equals(id), cancellationToken);
        }

        public async Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return await FindAll(predicate, includeProperties).FirstOrDefaultAsync(cancellationToken);
        }

        #endregion

        #region Write Operations

        public void Add(TEntity entity) => _dbSet.Add(entity);

        public void Update(TEntity entity) => _dbSet.Update(entity);

        public void Remove(TEntity entity) => _dbSet.Remove(entity);

        public async Task RemoveAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await FindByIdAsync(id, cancellationToken);
            if (entity != null) 
            {
                Remove(entity);
            }
        }

        public void RemoveMultiple(List<TEntity> entities) => _dbSet.RemoveRange(entities);

        #endregion
    }
}