using MedImage.Domain.Entities;
using MedImage.Domain.Interfaces;

namespace MedImage.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;

    public AuthService(IUnitOfWork uow, IPasswordHasher hasher)
    {
        _uow = uow;
        _hasher = hasher;
    }

    public async Task<AuthResult> RegisterAsync(string username, string password)
    {
        username = (username ?? string.Empty).Trim();
        if (username.Length < 3)
            return new AuthResult(false, "Username must be at least 3 characters.", null);
        if ((password ?? string.Empty).Length < 6)
            return new AuthResult(false, "Password must be at least 6 characters.", null);

        if (await _uow.Users.GetByUsernameAsync(username) is not null)
            return new AuthResult(false, "That username is already taken.", null);

        var (hash, salt) = _hasher.Hash(password!);
        var user = new User { Username = username, PasswordHash = hash, PasswordSalt = salt };
        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();
        return new AuthResult(true, null, user);
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        var user = await _uow.Users.GetByUsernameAsync((username ?? string.Empty).Trim());
        if (user is null || !_hasher.Verify(password ?? string.Empty, user.PasswordHash, user.PasswordSalt))
            return new AuthResult(false, "Invalid username or password.", null);
        return new AuthResult(true, null, user);
    }
}
