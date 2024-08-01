using System.Security.Cryptography;
using System.Text;

namespace PAMAi.Infrastructure.Identity;

/// <summary>
/// Utility class for encryption and decryption.
/// </summary>
internal static class Cryptography
{
    private static readonly byte[] s_key =
    [
        0x34, 0x30, 0x77, 0x37, 0x45, 0x46, 0x38, 0x52, 0x6a, 0x59, 0x47, 0x35, 0x31, 0x4f, 0x61, 0x44,
        0x57, 0x43, 0x47, 0x63, 0x30, 0x44, 0x6b, 0x75, 0x7a, 0x36, 0x39, 0x58, 0x44, 0x32, 0x44, 0x4e
    ];
    private static readonly byte[] s_iv =
    [
        0x37, 0x61, 0x59, 0x33, 0x63, 0x68, 0x55, 0x45, 0x74, 0x32, 0x4d, 0x52, 0x69, 0x44, 0x44, 0x4d,
    ];

    internal static string Encrypt(string text)
    {
        using Aes aes = Aes.Create();
        aes.Key = s_key;
        aes.IV = s_iv;
        byte[] textBytes = Encoding.UTF8.GetBytes(text);
        byte[] ciphertextBytes = aes.EncryptCbc(textBytes, s_iv);
        string ciphertext = Convert.ToBase64String(ciphertextBytes);

        return ciphertext;
    }

    internal static string Decrypt(string ciphertext)
    {
        using Aes aes = Aes.Create();
        aes.Key = s_key;
        aes.IV = s_iv;
        byte[] ciphertextBytes = Convert.FromBase64String(ciphertext);
        byte[] textBytes = aes.DecryptCbc(ciphertextBytes, s_iv);
        string text = Encoding.UTF8.GetString(textBytes);

        return text;
    }
}
