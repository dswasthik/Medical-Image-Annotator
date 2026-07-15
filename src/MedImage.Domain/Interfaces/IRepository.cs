using System.Linq.Expressions;

namespace MedImage.Domain.Interfaces;

// Generic repository abstraction (DIP: higher layers depend on this, not EF Core).
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
