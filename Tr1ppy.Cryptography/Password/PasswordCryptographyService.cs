using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Tr1ppy.Cryptography.Password;

public class PasswordCryptographyService(IOptions<HashPasswordSettings> options)
{
    private readonly HashPasswordSettings _settings = options.Value
        ?? throw new ArgumentNullException(nameof(options));

    public string HashPassword(string password)
    {
        return HashPassword(password, _settings);
    }

    public static string HashPassword(string password, HashPasswordSettings settings)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(settings.SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2
        (
            password: password,
            salt: salt,
            iterations: settings.IterationsCount,
            hashAlgorithm: settings.HashAlgorithm,
            outputLength: settings.HashSize
        );

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }


    public bool VerifyPassword(string password, string hashedPassword)
    {
        return VerifyPassword(password, hashedPassword, _settings);
    }

    public static bool VerifyPassword(string password, string hashedPassword, HashPasswordSettings settings)
    {

        string[] parts = hashedPassword.Split('.', 2);
        if (parts.Length != 2)
            return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] expectedHash = Convert.FromBase64String(parts[1]);

        var actualHash = Rfc2898DeriveBytes.Pbkdf2
        (
            password: password,
            salt: salt,
            iterations: settings.IterationsCount,
            hashAlgorithm: settings.HashAlgorithm,
            outputLength: settings.HashSize
        );

        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }
}


public class HashPasswordSettings
{
    public required HashAlgorithmName HashAlgorithm { get; set; }

    public required int HashSize { get; set; }

    public required int SaltSize { get; set; }

    public required int IterationsCount { get; set; }
}