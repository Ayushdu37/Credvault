using NUnit.Framework;
using CardService.Domain.Entities;

namespace CredVault.Tests.CardService;

[TestFixture]
public class CreditCardEntityTests
{
    // ─── Balance Logic ───────────────────────────────────────────────

    [Test]
    public void UpdateOutstandingBalance_PositiveAmount_IncreasesBalance()
    {
        // Arrange
        var card = CreditCard.Create(
            Guid.NewGuid(), "**** **** **** 1234", "hash123",
            "John Doe", 12, 2028, Guid.NewGuid(), 100000m, 1);

        // Act
        card.UpdateOutstandingBalance(5000m);

        // Assert
        Assert.That(card.OutstandingBalance, Is.EqualTo(5000m));
    }

    [Test]
    public void UpdateOutstandingBalance_NegativeAmount_DecreasesBalance()
    {
        // Arrange
        var card = CreditCard.Create(
            Guid.NewGuid(), "**** **** **** 1234", "hash123",
            "John Doe", 12, 2028, Guid.NewGuid(), 100000m, 1);
        card.UpdateOutstandingBalance(5000m);

        // Act
        card.UpdateOutstandingBalance(-2000m);

        // Assert
        Assert.That(card.OutstandingBalance, Is.EqualTo(3000m));
    }

    [Test]
    public void UpdateOutstandingBalance_NeverGoesBelowZero()
    {
        // Arrange
        var card = CreditCard.Create(
            Guid.NewGuid(), "**** **** **** 1234", "hash123",
            "John Doe", 12, 2028, Guid.NewGuid(), 100000m, 1);
        card.UpdateOutstandingBalance(1000m);

        // Act — paying more than the balance
        card.UpdateOutstandingBalance(-5000m);

        // Assert — Math.Max(0, 1000 + (-5000)) = 0
        Assert.That(card.OutstandingBalance, Is.EqualTo(0m));
    }

    [Test]
    public void AvailableCredit_EqualsLimitMinusBalance()
    {
        // Arrange
        var card = CreditCard.Create(
            Guid.NewGuid(), "**** **** **** 1234", "hash123",
            "John Doe", 12, 2028, Guid.NewGuid(), 100000m, 1);
        card.UpdateOutstandingBalance(30000m);

        // Act & Assert
        Assert.That(card.AvailableCredit, Is.EqualTo(70000m));
    }

    [TestCase(50000, 100000, 50.0)]
    [TestCase(0, 100000, 0.0)]
    [TestCase(100000, 100000, 100.0)]
    [TestCase(25000, 100000, 25.0)]
    public void CreditUtilization_CalculatedCorrectly(
        decimal balance, decimal limit, decimal expectedPercent)
    {
        // Arrange
        var card = CreditCard.Create(
            Guid.NewGuid(), "**** **** **** 1234", "hash123",
            "John Doe", 12, 2028, Guid.NewGuid(), limit, 1);
        card.UpdateOutstandingBalance(balance);

        // Act
        var utilization = limit > 0
            ? Math.Round(card.OutstandingBalance / card.CreditLimit * 100, 2)
            : 0;

        // Assert
        Assert.That(utilization, Is.EqualTo(expectedPercent));
    }

    // ─── State Changes ───────────────────────────────────────────────

    [Test]
    public void SetAsDefault_SetsIsDefaultToTrue()
    {
        // Arrange
        var card = CreditCard.Create(
            Guid.NewGuid(), "**** 1234", "hash", "Jane", 6, 2029, Guid.NewGuid(), 50000m, 1);

        // Act
        card.SetAsDefault();

        // Assert
        Assert.That(card.IsDefault, Is.True);
    }

    [Test]
    public void UnsetDefault_SetsIsDefaultToFalse()
    {
        // Arrange
        var card = CreditCard.Create(
            Guid.NewGuid(), "**** 1234", "hash", "Jane", 6, 2029, Guid.NewGuid(), 50000m, 1);
        card.SetAsDefault();

        // Act
        card.UnsetDefault();

        // Assert
        Assert.That(card.IsDefault, Is.False);
    }

    [Test]
    public void SoftDelete_SetsIsDeletedAndDeletedAt()
    {
        // Arrange
        var card = CreditCard.Create(
            Guid.NewGuid(), "**** 1234", "hash", "Jane", 6, 2029, Guid.NewGuid(), 50000m, 1);

        // Act
        card.SoftDelete();

        // Assert
        Assert.That(card.IsDeleted, Is.True);
        Assert.That(card.DeletedAt, Is.Not.Null);
    }

    [Test]
    public void Verify_SetsIsVerifiedToTrue()
    {
        // Arrange
        var card = CreditCard.Create(
            Guid.NewGuid(), "**** 1234", "hash", "Jane", 6, 2029, Guid.NewGuid(), 50000m, 1);

        // Act
        card.Verify();

        // Assert
        Assert.That(card.IsVerified, Is.True);
    }

    [Test]
    public void UpdateCreditLimit_ChangesLimit()
    {
        // Arrange
        var card = CreditCard.Create(
            Guid.NewGuid(), "**** 1234", "hash", "Jane", 6, 2029, Guid.NewGuid(), 50000m, 1);

        // Act
        card.UpdateCreditLimit(150000m);

        // Assert
        Assert.That(card.CreditLimit, Is.EqualTo(150000m));
    }

    [Test]
    public void Create_SetsInitialValuesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var issuerId = Guid.NewGuid();

        // Act
        var card = CreditCard.Create(userId, "**** 5678", "hash5678",
            "Test User", 3, 2027, issuerId, 80000m, 15);

        // Assert
        Assert.That(card.UserId, Is.EqualTo(userId));
        Assert.That(card.IssuerId, Is.EqualTo(issuerId));
        Assert.That(card.OutstandingBalance, Is.EqualTo(0m));
        Assert.That(card.IsDefault, Is.False);
        Assert.That(card.IsVerified, Is.False);
        Assert.That(card.IsDeleted, Is.False);
        Assert.That(card.BillingCycleStartDay, Is.EqualTo(15));
    }
}
