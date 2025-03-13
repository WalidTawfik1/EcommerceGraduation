using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Interfaces
{
    public interface IGenericRepository<T,TId> where T : class
    {

        Task<IReadOnlyList<T>> GetAllAsync();

        Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

        Task<T> GetByIdAsync(TId id);

        Task<T> GetByIdAsync(TId id, params Expression<Func<T, object>>[] includes);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(TId id);

        Task<int> CountAsync();
    }
}
