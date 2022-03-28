using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using RainyCorp.UserManagerService.Entities.Interfaces;
using RainyCorp.UserManagerService.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RainyCorp.UserManagerService.Repository
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private bool _disposed;
        private readonly TContext _context;
        private readonly ILogger<UnitOfWork<TContext>> _logger;


        public UnitOfWork(TContext dbContext, ILogger<UnitOfWork<TContext>> logger)
        {
            _context = dbContext;
            _logger = logger;
        }


        public virtual List<T> Get<T>() where T : class, IBaseEntity => _context.Set<T>().ToList();


        public virtual List<T> Get<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity => Query(expression).ToList();


        public virtual void Add<T>(T entity) where T : class, IBaseEntity
        {
            entity.CreatedAt = DateTime.Now;
            _context.Add(entity);
        }

        public Task<List<T>> GetAsync<T>() where T : class, IBaseEntity => _context.Set<T>().ToListAsync();
        public Task<T> GetAsync<T>(long id) where T : class, IBaseEntity => _context.Set<T>().FirstOrDefaultAsync();


        public async Task<T> AddAsync<T>(T entity) where T : class, IBaseEntity
        {
            entity.CreatedAt = DateTime.Now;
            await _context.AddAsync(entity);
            return entity;
        }

        public virtual void Update<T>(T entity) where T : class, IBaseEntity
        {
        
            _context.Update(entity);
        }

        /// <summary>
        /// Updates the range asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateRange<T>(IEnumerable<T> entities) where T : class, IBaseEntity => _context.UpdateRange(entities);


        public virtual void Delete<T>(T entity) where T : class, IBaseEntity => _context.Remove(entity);

        public Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity =>
            _context.Set<T>().Where(expression).ToListAsync();

        //public virtual void SoftDelete<T>(T entity) where T : class, IBaseEntity
        //{
        //    entity.IsDeleted = true;
        //    entity.UpdatedAt = DateTime.Now;
        //    Update(entity);
        //}


        public virtual T FirstOrDefault<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity =>
            Query(expression).FirstOrDefault();


        public virtual Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression)
            where T : class, IBaseEntity => Query(expression).FirstOrDefaultAsync();


        public virtual IQueryable<T> Query<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity => _context.Set<T>().Where(expression);


        public virtual IQueryable<T> Query<T>() where T : class, IBaseEntity => _context.Set<T>();


        public Task<bool> AnyAsync<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity =>
            _context.Set<T>().AnyAsync(expression);

        public void Save() => _context.SaveChanges();
        public virtual Task SaveAsync() => _context.SaveChangesAsync();

        public Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, IBaseEntity => _context.AddRangeAsync(entities);

        /// <summary>
        /// Executes the in transaction asynchronously.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns></returns>
        public virtual Task ExecuteInTransactionAsync(Func<IUnitOfWork, Task> action, IsolationLevel isolationLevel)
        {
            IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();

            return strategy.ExecuteAsync(async () =>
            {
                using (IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync(isolationLevel))
                {
                    try
                    {
                        // Execute the action itself
                        await action(this);

                        // save changes.
                        await SaveAsync();

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Transaction failed while creating an execution strategy.");
                        _logger.LogWarning("Trying to rollback...");
                        transaction.Rollback();
                        _logger.LogWarning("Rollback successfully!");
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// Executes the in transaction asynchronously.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public virtual Task ExecuteInTransactionAsync(Func<IUnitOfWork, Task> action) => ExecuteInTransactionAsync(action, IsolationLevel.ReadCommitted);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _disposed = true;
                _context.Dispose();
            }
        }
    }
}
