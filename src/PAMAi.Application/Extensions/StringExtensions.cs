using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace PAMAi.Application.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Computes a SHA-512 hash for the string and returns the hashed value as a Base64-URL encoded string.
    /// </summary>
    /// <param name="source">String to hash.</param>
    /// <returns>A SHA-512 Base64-URL encoded string.</returns>
    public static string ToSha512Base64UrlEncoding(this string source)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(source);
        byte[] hashBytes = SHA512.HashData(bytes);

        return Base64UrlTextEncoder.Encode(hashBytes);
    }
}
