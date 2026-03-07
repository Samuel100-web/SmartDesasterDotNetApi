using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SmartResponse.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        // Naya Method: Taake hum Incident ke sath uski Media aur Type bhi nikal saken
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity); // Soft delete DbContext handle karega
    }

}
