using MedImage.Domain.Entities;
using MedImage.Domain.Interfaces;

namespace MedImage.Infrastructure.Services;

public class SessionService : ISessionService
{
    public User? CurrentUser { get; set; }
    public bool IsAuthenticated => CurrentUser is not null;
}
