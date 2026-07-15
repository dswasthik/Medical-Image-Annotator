using MedImage.Domain.Interfaces;
using MedImage.Infrastructure.Data.Repositories;

namespace MedImage.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    public IUserRepository Users { get; }
    public IStudyRepository Studies { get; }

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Users = new UserRepository(db);
        Studies = new StudyRepository(db);
    }

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
}
