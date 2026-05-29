using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToyStoreManagement.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
        Task AddRangeAsync(IEnumerable<T> entities);
    }
}
