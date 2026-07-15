using MedImage.Domain.Entities;

namespace MedImage.Domain.Interfaces;

// Holds the currently authenticated user for the app session.
public interface ISessionService
{
    User? CurrentUser { get; set; }
    bool IsAuthenticated { get; }
}
