using PAMAi.Application.Extensions;
using PAMAi.Infrastructure.Identity.Models;

namespace PAMAi.Tests.Unit.Infrastructure.Identity.Models;

internal class RefreshTokenTests
{
    [Test]
    public void Should_BeRevoked()
    {
        RefreshToken token = new()
        {
            CreatedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = DateTimeOffset.UtcNow,
            RevokedUtc = null,
            Token = "qwerty".ToSha512Base64UrlEncoding(),
        };
        RefreshToken revokedToken = new()
        {
            CreatedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(-1),
            RevokedUtc = DateTimeOffset.UtcNow,
            Token = "qwerty".ToSha512Base64UrlEncoding(),
        };

        Assert.Multiple(() =>
        {
            Assert.That(token.IsRevoked, Is.False, "Refresh token is wrongly indicated as revoked.");
            Assert.That(revokedToken.IsRevoked, Is.True, "Refresh token is not indicated to be revoked.");
        });
    }

    [Test]
    public void Should_BeExpired()
    {
        RefreshToken token = new()
        {
            CreatedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
            RevokedUtc = null,
            Token = "qwerty".ToSha512Base64UrlEncoding(),
        };
        RefreshToken expiredToken = new()
        {
            CreatedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(-100),
            RevokedUtc = null,
            Token = "qwerty".ToSha512Base64UrlEncoding(),
        };

        Assert.Multiple(() =>
        {
            Assert.That(token.IsExpired, Is.False, "Refresh token is wrongly indicated as expired.");
            Assert.That(expiredToken.IsExpired, Is.True, "Refresh token is not indicated to be expired.");
        });
    }

    [Test]
    public void Should_BeActive()
    {
        RefreshToken token = new()
        {
            CreatedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
            RevokedUtc = null,
            Token = "qwerty".ToSha512Base64UrlEncoding(),
        };
        RefreshToken expiredToken = new()
        {
            CreatedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(-100),
            RevokedUtc = null,
            Token = "qwerty".ToSha512Base64UrlEncoding(),
        };
        RefreshToken revokedToken = new()
        {
            CreatedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(5),
            RevokedUtc = DateTimeOffset.UtcNow,
            Token = "qwerty".ToSha512Base64UrlEncoding(),
        };

        Assert.Multiple(() =>
        {
            Assert.That(token.IsActive, Is.True, "Refresh token is not indicated to be active.");
            Assert.That(expiredToken.IsActive, Is.False, "Refresh token is wrongly indicated as expired.");
            Assert.That(revokedToken.IsActive, Is.False, "Refresh token is wrongly indicated as expired.");
        });
    }
}
