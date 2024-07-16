using System.Security.Cryptography;

namespace PAMAi.Application.Helpers;

/// <summary>
/// Helper class for string operations and methods.
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Creates a secure random string of a given length.
    /// </summary>
    /// <param name="length">Length of the string to be generated.</param>
    /// <returns>A secure randomly-generated string.</returns>
    /// <exception cref="ArgumentException">Thrown for invalid values of <paramref name="length"/>.</exception>
    public static string CreateSecureRandomString(int length = 40)
    {
        if (length < 1)
            throw new ArgumentException("Invalid length. Length must be greater than zero (0).", nameof(length));

        const string allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        byte[] randomBytes = RandomNumberGenerator.GetBytes(length);
        char[] randomChars = new char[length];

        for (int i = 0; i < length; i++)
        {
            // Fill the random characters only with allowed characters.
            randomChars[i] = allowedCharacters[randomBytes[i] % allowedCharacters.Length];
        }

        return new string(randomChars);
    }
}
