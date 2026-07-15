using System.Security.Cryptography;
using MedImage.Domain.Interfaces;

namespace MedImage.Infrastructure.Security;

// PBKDF2 (SHA-256) hasher using only the BCL. No third-party crypto needed.
public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;      // 128-bit
    private const int KeySize = 32;       // 256-bit
    private const int Iterations = 100_000;

    public (string Hash, string Salt) Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] key = Derive(password, salt);
        return (Convert.ToBase64String(key), Convert.ToBase64String(salt));
    }

    public bool Verify(string password, string hash, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        byte[] expected = Convert.FromBase64String(hash);
        byte[] actual = Derive(password, saltBytes);
        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }

    private static byte[] Derive(string password, byte[] salt) =>
        Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
}
