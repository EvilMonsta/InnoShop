using System.Security.Cryptography;
using System.Text;

namespace InnoShop.Users.Infrastructure.Security;

public static class TokenGenerator
{
    public static (string Raw, string HashBase64) CreateSecureToken(int bytes = 32)
    {
        var raw = new byte[bytes];
        RandomNumberGenerator.Fill(raw);
        var rawB64 = Base64UrlEncode(raw);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(rawB64));
        var hashB64 = Convert.ToBase64String(hash);
        return (rawB64, hashB64);
    }

    public static string Base64UrlEncode(ReadOnlySpan<byte> bytes)
        => Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
}
