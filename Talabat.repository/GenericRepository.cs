using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.core.Entities;
using Talabat.core.IRepositories;
using Talabat.core.Specifications;
using Talabat.repository.Data;

namespace Talabat.repository
{
    
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _context;

        public GenericRepository(StoreContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await _context.Set<T>().ToListAsync();

        public async Task<T> GetByIdAsync(int id)
            => await _context.Set<T>().FindAsync(id);

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
            => await ApplySpecifications(spec).ToListAsync();

        public async Task<T> GetByIdWithSpecAsync(ISpecification<T> spec)
            => await ApplySpecifications(spec).FirstOrDefaultAsync();

        private IQueryable<T> ApplySpecifications(ISpecification<T> spec)
            => SpecificationEvaluator<T>.GetQuery(_context.Set<T>(), spec);

        public async Task<int> GetCountAsync(ISpecification<T> spec)
        {
            return await ApplySpecifications(spec).CountAsync();
        }

        public async Task CreateAsync(T entity)
            => await _context.Set<T>().AddAsync(entity);

        public void Update(T entity)
            => _context.Set<T>().Update(entity);

        public void Delete(T entity)
            => _context.Set<T>().Remove(entity);
    }
}
