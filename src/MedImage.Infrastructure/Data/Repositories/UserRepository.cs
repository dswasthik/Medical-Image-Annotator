using MedImage.Domain.Entities;
using MedImage.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedImage.Infrastructure.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext db) : base(db) { }

    public async Task<User?> GetByUsernameAsync(string username) =>
        await Set.FirstOrDefaultAsync(u => u.Username == username);
}
