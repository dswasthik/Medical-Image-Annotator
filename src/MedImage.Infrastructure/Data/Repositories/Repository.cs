using System.Linq.Expressions;
using MedImage.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedImage.Infrastructure.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Db;
    protected DbSet<T> Set => Db.Set<T>();

    public Repository(AppDbContext db) => Db = db;

    public async Task<T?> GetByIdAsync(int id) => await Set.FindAsync(id);

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null)
    {
        IQueryable<T> q = Set.AsNoTracking();
        if (predicate is not null) q = q.Where(predicate);
        return await q.ToListAsync();
    }

    public async Task AddAsync(T entity) => await Set.AddAsync(entity);
    public void Update(T entity) => Set.Update(entity);
    public void Remove(T entity) => Set.Remove(entity);
}
