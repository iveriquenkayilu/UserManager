using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UserManagerService.Entities.Interfaces;

namespace UserManagerService.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        void Add<T>(T entity) where T : class, IBaseEntity;

        Task<List<T>> GetAsync<T>() where T : class, IBaseEntity;
        Task<T> GetAsync<T>(Guid id) where T : class, IBaseEntity;
        Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity;

        Task<T> AddAsync<T>(T entity) where T : class, IBaseEntity;

        void Update<T>(T entity) where T : class, IBaseEntity;
        void UpdateRange<T>(IEnumerable<T> entities) where T : class, IBaseEntity;

        void Delete<T>(T entity) where T : class, IBaseEntity;

        //void SoftDelete<T>(T entity) where T : class, IBaseEntity;

        List<T> Get<T>() where T : class, IBaseEntity;

        List<T> Get<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity;

        T FirstOrDefault<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity;

        Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity;

        IQueryable<T> Query<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity; // not used

        IQueryable<T> Query<T>() where T : class, IBaseEntity;  //not used 
        Task<bool> AnyAsync<T>(Expression<Func<T, bool>> expression) where T : class, IBaseEntity;

        void Save();
        Task SaveAsync();

        Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, IBaseEntity;

        Task ExecuteInTransactionAsync(Func<IUnitOfWork, Task> action);
        Task SoftDeleteEntityAsync<T>(Guid id) where T : class, IBaseEntity;
        Task SoftDeleteEntityAsync<T>(Guid id, Guid userId) where T : class, IBaseEntity;
    }
}
