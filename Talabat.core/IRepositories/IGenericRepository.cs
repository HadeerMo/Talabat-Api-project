using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.core.Entities;
using Talabat.core.Specifications;

namespace Talabat.core.IRepositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);

        Task<T> GetByIdWithSpecAsync(ISpecification<T> spec);
        Task<int> GetCountAsync(ISpecification<T> spec);

        Task CreateAsync(T entity);
        void Update(T entity);  
        void Delete(T entity);
    }
}
