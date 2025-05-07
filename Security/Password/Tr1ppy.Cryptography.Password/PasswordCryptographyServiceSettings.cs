using System.Security.Cryptography;

namespace Tr1ppy.Cryptography.Password;

public class PasswordCryptographyServiceSettings
{
    public HashAlgorithmName HashAlgorithm { get; set; }

    public int HashSize { get; set; }

    public int SaltSize { get; set; }

    public int IterationsCount { get; set; }
}
