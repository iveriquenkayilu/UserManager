using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UserManagerService.Entities.Interfaces;
using UserManagerService.Interfaces.Repositories;
using UserManagerService.Shared.Exceptions;
using UserManagerService.Shared.Interfaces.Services;

namespace UserManagerService.Repository
{
	public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
	{
		private bool _disposed;
		private readonly TContext _context;
		private readonly IUserContext _userContext;
		private readonly ILogger<UnitOfWork<TContext>> _logger;

		public UnitOfWork(TContext dbContext, ILogger<UnitOfWork<TContext>> logger, IUserContext userContext)
		{
			_context = dbContext;
			_logger = logger;
			_userContext = userContext;
		}


		public virtual List<T> Get<T>() where T : class, IBaseEntity => _context.Set<T>().ToList();


		public virtual List<T> Get<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity => Query(expression).ToList();


		public virtual void Add<T>(T entity) where T : class, IBaseEntity
		{
			entity.CreatedAt = DateTime.Now;
			_context.Add(entity);
		}

		public Task<List<T>> GetAsync<T>() where T : class, IBaseEntity => _context.Set<T>().ToListAsync();
		public Task<T> GetAsync<T>(Guid id) where T : class, IBaseEntity => _context.Set<T>().FirstOrDefaultAsync();


		public async Task<T> AddAsync<T>(T entity) where T : class, IBaseEntity
		{
			entity.CreatedAt = DateTime.Now;
			if (entity.CreatorId == Guid.Empty)
				entity.CreatorId = _userContext.UserId;

			await _context.AddAsync(entity);
			return entity;
		}

		public async Task<T> AddToCompanyAsync<T>(T entity) where T : class, IBaseCompanyEntity
		{
			entity.CreatedAt = DateTime.Now;
			if (entity.CreatorId == Guid.Empty)
				entity.CreatorId = _userContext.UserId;
			if (entity.CompanyId == Guid.Empty)
				entity.CompanyId = _userContext.CompanyId;

			await _context.AddAsync(entity);
			return entity;
		}

		public virtual void Update<T>(T entity) where T : class, IBaseEntity
		{

			_context.Update(entity);
		}

		public void UpdateRange<T>(IEnumerable<T> entities) where T : class, IBaseEntity => _context.UpdateRange(entities);

		public virtual void Delete<T>(T entity) where T : class, IBaseEntity => _context.Remove(entity);

		public Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity =>
			_context.Set<T>().Where(expression).ToListAsync();
		public virtual void SoftDelete<T>(T entity) where T : class, IBaseEntity
		{
			entity.UpdatedAt = DateTime.Now;
			entity.DeletedAt = DateTime.Now;
			Update(entity);
		}

		public async Task SoftDeleteEntityAsync<T>(Guid id) where T : class, IBaseEntity
		{
			var documentType = await Query<T>(d => d.Id == id).FirstOrDefaultAsync();

			if (documentType is null)
				throw new CustomException($"{nameof(T)} {id} not found");

			SoftDelete(documentType);
			await SaveAsync();
		}

		public async Task SoftDeleteEntityAsync<T>(Guid id, Guid userId) where T : class, IBaseEntity
		{
			var documentType = await Query<T>(d => d.Id == id).FirstOrDefaultAsync();

			if (documentType is null)
				throw new CustomException($"{nameof(T)} {id} not found");

			//documentType.UpdatedBy = userId;

			SoftDelete(documentType);
			await SaveAsync();
		}

		public virtual T FirstOrDefault<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity =>
			Query(expression).FirstOrDefault();

		public virtual Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression)
			where T : class, IBaseEntity => Query(expression).FirstOrDefaultAsync();

		public virtual IQueryable<T> Query<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity => _context.Set<T>().Where(e => e.DeletedAt == null).Where(expression);


		public virtual IQueryable<T> Query<T>() where T : class, IBaseEntity => Query<T>(e => true);
		public virtual IQueryable<T> QueryByCompanyId<T>(Expression<Func<T, bool>> expression) where T : class, IBaseCompanyEntity
		  => _context.Set<T>().Where(e => e.DeletedAt == null && e.CompanyId == _userContext.CompanyId).Where(expression);

		public virtual IQueryable<T> QueryByCompanyId<T>() where T : class, IBaseCompanyEntity
		  => QueryByCompanyId<T>(e => true);

		public Task<bool> AnyAsync<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity =>
			Query<T>().AnyAsync(expression);

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
