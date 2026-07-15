using MedImage.Domain.Entities;

namespace MedImage.Domain.Interfaces;

public record AuthResult(bool Success, string? Error, User? User);

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string username, string password);
    Task<AuthResult> LoginAsync(string username, string password);
}
