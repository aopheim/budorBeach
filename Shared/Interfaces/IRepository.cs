using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken);
        Task AddAsync(T entity, CancellationToken cancellationToken);
        Task UpdateAsync(T entity, CancellationToken cancellationToken);
        void UpdateRange(IEnumerable<T> entities, CancellationToken cancellationToken);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);
        Task RemoveAsync(T entity, CancellationToken cancellationToken);
        void RemoveRange(IEnumerable<T> entities, CancellationToken cancellationToken);
    }

    public interface IRepositories
    {
        ISensorReadingRepo SensorReadings { get; }
        IBirdPresenceRepo BirdPresenceRegistrations { get; }
        ISpeciesRecognitionRepo SpeciesRecognitions { get; }
        IImageUploadRepo ImageUploads { get; set; }
        IHiddenSpeciesRepo HiddenSpecies { get; set; }
        public Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}