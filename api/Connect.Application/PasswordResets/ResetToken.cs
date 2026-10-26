using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Connect.Application.PasswordResets;

// The token behind a one time link, such as a parent's permission link (#392) or a password
// reset link (#388). Only its hash is ever stored, as the OWASP Forgot Password Cheat Sheet asks.
public static class ResetToken
{
    private const int TokenBytes = 32;

    public static string Make()
    {
        return Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(TokenBytes));
    }

    public static string Hash(string token)
    {
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
