namespace MedImage.Domain.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IStudyRepository Studies { get; }
    Task<int> SaveChangesAsync();
}
