using System.Diagnostics;
using PAMAi.Infrastructure.Identity;

namespace PAMAi.Tests.Unit.Infrastructure.Identity;

internal class CryptographyTests: BaseTest
{
    [Test]
    public void Should_EncryptAndDecrypt()
    {
        string text = "A couple of 3 letters and one symbol!";
        var ciphertext = Cryptography.Encrypt(text);
        Trace.WriteLine($"ciphertext: {ciphertext}");
        string decryptedText = Cryptography.Decrypt(ciphertext);

        Assert.That(decryptedText, Is.EqualTo(text));
    }
}
