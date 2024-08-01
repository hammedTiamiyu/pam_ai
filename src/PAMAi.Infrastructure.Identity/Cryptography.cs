using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace PAMAi.Infrastructure.Identity;

/// <summary>
/// Utility class for encryption and decryption.
/// </summary>
internal static class Cryptography
{
    private const int IV_LENGTH = 16;

    private static readonly byte[] s_key =
    [
        0x34, 0x30, 0x77, 0x37, 0x45, 0x46, 0x38, 0x52, 0x6a, 0x59, 0x47, 0x35, 0x31, 0x4f, 0x61, 0x44,
        0x57, 0x43, 0x47, 0x63, 0x30, 0x44, 0x6b, 0x75, 0x7a, 0x36, 0x39, 0x58, 0x44, 0x32, 0x44, 0x4e
    ];
    //private static readonly byte[] s_iv =
    //[
    //    0x37, 0x61, 0x59, 0x33, 0x63, 0x68, 0x55, 0x45, 0x74, 0x32, 0x4d, 0x52, 0x69, 0x44, 0x44, 0x4d,
    //];

    internal static string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = s_key;
        byte[] textBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] iv = GenerateIV();
        byte[] ciphertextBytes = aes.EncryptCbc(textBytes, iv);
        byte[] result = CombineCiphertextAndIv(ciphertextBytes, iv);
        string ciphertext = Convert.ToBase64String(result);

        return ciphertext;
    }

    internal static string Decrypt(string encryptedText)
    {
        using Aes aes = Aes.Create();
        aes.Key = s_key;
        byte[] result = Convert.FromBase64String(encryptedText);
        (byte[] ciphertext, byte[] iv) = SplitCiphertextAndIv(result);
        Debug.Assert(iv.Length == IV_LENGTH);
        byte[] textBytes = aes.DecryptCbc(ciphertext, iv);
        string text = Encoding.UTF8.GetString(textBytes);

        return text;
    }

    private static byte[] GenerateIV()
    {
        byte[] randomBytes = new byte[IV_LENGTH];
        RandomNumberGenerator.Fill(randomBytes);
        
        return randomBytes;
    }

    private static byte[] CombineCiphertextAndIv(byte[] ciphertext, byte[] iv)
    {
        return [..iv, ..ciphertext];
    }

    private static (byte[] ciphertext, byte[] iv) SplitCiphertextAndIv(byte[] bytes)
    {
        if (bytes.Length < IV_LENGTH)
            throw new ArgumentException($"Length of bytes must be greater than {IV_LENGTH}", nameof(bytes));

        return (bytes[IV_LENGTH..], bytes[..IV_LENGTH]);
    }
}
