using MedImage.Domain.Entities;

namespace MedImage.Domain.Interfaces;

public interface IStudyRepository : IRepository<Study>
{
    Task<IReadOnlyList<Study>> GetForUserAsync(int userId);
    Task<Study?> GetWithAnnotationsAsync(int studyId);
}
