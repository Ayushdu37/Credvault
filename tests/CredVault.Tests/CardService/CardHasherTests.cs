using NUnit.Framework;
using CardService.Infrastructure.Services;

namespace CredVault.Tests.CardService;

[TestFixture]
public class CardHasherTests
{
    private CardHasher _hasher = null!;

    [SetUp]
    public void SetUp()
    {
        _hasher = new CardHasher();
    }

    [Test]
    public void HashCardNumber_ReturnsNonPlaintextString()
    {
        // Arrange
        var cardNumber = "4111111111111234";

        // Act
        var hash = _hasher.HashCardNumber(cardNumber);

        // Assert
        Assert.That(hash, Is.Not.EqualTo(cardNumber));
        Assert.That(hash.Length, Is.GreaterThan(0));
    }

    [Test]
    public void HashCardNumber_SameInputProducesSameHash()
    {
        // Arrange — SHA256 is deterministic
        var cardNumber = "4111111111111234";

        // Act
        var hash1 = _hasher.HashCardNumber(cardNumber);
        var hash2 = _hasher.HashCardNumber(cardNumber);

        // Assert
        Assert.That(hash1, Is.EqualTo(hash2));
    }

    [Test]
    public void HashCardNumber_DifferentInputProducesDifferentHash()
    {
        // Arrange & Act
        var hash1 = _hasher.HashCardNumber("4111111111111234");
        var hash2 = _hasher.HashCardNumber("5500000000000004");

        // Assert
        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }

    [Test]
    public void MaskCardNumber_ReturnsCorrectFormat()
    {
        // Arrange
        var cardNumber = "4111111111111234";

        // Act
        var masked = _hasher.MaskCardNumber(cardNumber);

        // Assert
        Assert.That(masked, Is.EqualTo("**** **** **** 1234"));
    }

    [Test]
    public void MaskCardNumber_HandlesSpaces()
    {
        // Arrange
        var cardNumber = "4111 1111 1111 5678";

        // Act
        var masked = _hasher.MaskCardNumber(cardNumber);

        // Assert
        Assert.That(masked, Is.EqualTo("**** **** **** 5678"));
    }

    [Test]
    public void MaskCardNumber_PreservesLast4Digits()
    {
        // Arrange
        var cardNumber = "6221260012345678";

        // Act
        var masked = _hasher.MaskCardNumber(cardNumber);

        // Assert
        Assert.That(masked, Does.EndWith("5678"));
    }
}
