using Microsoft.IdentityModel.Tokens;

namespace PAMAi.Infrastructure.Identity;

/// <summary>
/// Contains constants used for identity in the application.
/// </summary>
internal static class Constants
{
    /// <summary>
    /// Password requirements.
    /// </summary>
    internal static class PasswordRequirements
    {
        /// <summary>
        /// Password minimum length.
        /// </summary>
        public static readonly int MinLength = 8;

        /// <summary>
        /// Indicates if the password must contain alpha-numeric characters.
        /// </summary>
        public static readonly bool RequireNonAlphanumeric = true;

        /// <summary>
        /// Indicates if the password must contain upper-case characters.
        /// </summary>
        public static readonly bool RequireUppercase = true;

        /// <summary>
        /// Indicates if the password must contain lower-case characters.
        /// </summary>
        public static readonly bool RequireLowercase = true;

        /// <summary>
        /// Indicates if the password must contain digits.
        /// </summary>
        public static readonly bool RequireDigit = true;
    }

    internal static class Jwt
    {
        /// <summary>
        /// JWT key.
        /// </summary>
        public static readonly byte[] Key =
        [
            0x7a, 0x61, 0x47, 0x72, 0x64, 0x51, 0x38, 0x74,
            0x74, 0x6d, 0x68, 0x68, 0x53, 0x6c, 0x71, 0x53,
            0x41, 0x71, 0x50, 0x35, 0x6f, 0x4d, 0x34, 0x78,
            0x46, 0x49, 0x41, 0x70, 0x59, 0x35, 0x41, 0x50
        ];

        /// <summary>
        /// Symmetric security key for JWTs.
        /// </summary>
        public static readonly SymmetricSecurityKey SecurityKey = new(Key);

        /// <summary>
        /// Valid algorithms for signing JWTs in the application.
        /// </summary>
        public static readonly List<string> Algorithms = [SecurityAlgorithms.HmacSha256];
    }
}
