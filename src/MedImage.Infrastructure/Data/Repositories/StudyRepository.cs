using MedImage.Domain.Entities;
using MedImage.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedImage.Infrastructure.Data.Repositories;

public class StudyRepository : Repository<Study>, IStudyRepository
{
    public StudyRepository(AppDbContext db) : base(db) { }

    public async Task<IReadOnlyList<Study>> GetForUserAsync(int userId) =>
        await Set.AsNoTracking()
                 .Where(s => s.UserId == userId)
                 .OrderByDescending(s => s.CreatedAt)
                 .ToListAsync();

    public async Task<Study?> GetWithAnnotationsAsync(int studyId) =>
        await Set.Include(s => s.Annotations)
                 .FirstOrDefaultAsync(s => s.Id == studyId);
}
