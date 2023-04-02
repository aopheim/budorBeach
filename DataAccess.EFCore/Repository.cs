using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces;

namespace DataAccess.EFCore
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly BudorDbContext _context;

        protected Repository(BudorDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Set<T>().FindAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> expression,
            CancellationToken cancellationToken)
        {
            return await _context.Set<T>().Where(expression).ToListAsync(cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            await _context.Set<T>().AddRangeAsync(entities, cancellationToken);
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            return Task.FromResult(_context.Set<T>().Update(entity));
        }

        public void UpdateRange(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            _context.Set<T>().UpdateRange(entities);
        }

        public Task RemoveAsync(T entity, CancellationToken cancellationToken)
        {
            return Task.FromResult(_context.Set<T>().Remove(entity));
        }

        public void RemoveRange(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            _context.Set<T>().RemoveRange(entities);
        }
    }
}