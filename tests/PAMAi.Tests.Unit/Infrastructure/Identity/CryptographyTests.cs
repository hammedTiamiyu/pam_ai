using System.Diagnostics;
using PAMAi.Infrastructure.Identity;

namespace PAMAi.Tests.Unit.Infrastructure.Identity;

internal class CryptographyTests: BaseTest
{
    [TestCase("A couple of 3 letters and one symbol!", Description = "This exact string is run twice to test if the encrypted value will be unique despite being the same string.")]
    [TestCase("A couple of 3 letters and one symbol!", Description = "This exact string is run twice to test if the encrypted value will be unique despite being the same string.")]
    [TestCase("$ecret P@ssw0RD")]
    public void Should_EncryptAndDecrypt(string text)
    {
        var ciphertext = Cryptography.Encrypt(text);
        Trace.WriteLine($"Encrypted '{text}': {ciphertext}");
        string decryptedText = Cryptography.Decrypt(ciphertext);

        Assert.That(decryptedText, Is.EqualTo(text));
    }
}
