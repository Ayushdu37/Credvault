using NUnit.Framework;
using BillingService.Domain.Entities;

namespace CredVault.Tests.BillingService;

[TestFixture]
public class PaymentScheduleEntityTests
{
    [Test]
    public void Create_SetsDefaultStatusToPending()
    {
        // Arrange & Act
        var schedule = PaymentSchedule.Create(
            Guid.NewGuid(), Guid.NewGuid(), 5000m, DateTime.UtcNow.AddDays(7));

        // Assert
        Assert.That(schedule.Status, Is.EqualTo("Pending"));
        Assert.That(schedule.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(schedule.Amount, Is.EqualTo(5000m));
    }

    [Test]
    public void MarkExecuted_SetsStatusAndUpdatedAt()
    {
        // Arrange
        var schedule = PaymentSchedule.Create(
            Guid.NewGuid(), Guid.NewGuid(), 5000m, DateTime.UtcNow.AddDays(7));

        // Act
        schedule.MarkExecuted();

        // Assert
        Assert.That(schedule.Status, Is.EqualTo("Executed"));
        Assert.That(schedule.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public void Cancel_SetsStatusAndUpdatedAt()
    {
        // Arrange
        var schedule = PaymentSchedule.Create(
            Guid.NewGuid(), Guid.NewGuid(), 5000m, DateTime.UtcNow.AddDays(7));

        // Act
        schedule.Cancel();

        // Assert
        Assert.That(schedule.Status, Is.EqualTo("Cancelled"));
        Assert.That(schedule.UpdatedAt, Is.Not.Null);
    }
}

[TestFixture]
public class RewardAccountEntityTests
{
    [Test]
    public void Create_SetsZeroPoints()
    {
        // Arrange & Act
        var account = RewardAccount.Create(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.That(account.AvailablePoints, Is.EqualTo(0));
        Assert.That(account.TotalEarned, Is.EqualTo(0));
    }

    [Test]
    public void EarnPoints_IncreasesAvailableAndTotal()
    {
        // Arrange
        var account = RewardAccount.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act
        account.EarnPoints(500);

        // Assert
        Assert.That(account.AvailablePoints, Is.EqualTo(500));
        Assert.That(account.TotalEarned, Is.EqualTo(500));
    }

    [Test]
    public void EarnPoints_MultipleEarns_Accumulates()
    {
        // Arrange
        var account = RewardAccount.Create(Guid.NewGuid(), Guid.NewGuid());
        account.EarnPoints(200);
        account.EarnPoints(300);

        // Assert
        Assert.That(account.AvailablePoints, Is.EqualTo(500));
        Assert.That(account.TotalEarned, Is.EqualTo(500));
    }

    [Test]
    public void CanRedeem_SufficientPoints_ReturnsTrue()
    {
        // Arrange
        var account = RewardAccount.Create(Guid.NewGuid(), Guid.NewGuid());
        account.EarnPoints(1000);

        // Assert
        Assert.That(account.CanRedeem(500), Is.True);
        Assert.That(account.CanRedeem(1000), Is.True);
    }

    [Test]
    public void CanRedeem_InsufficientPoints_ReturnsFalse()
    {
        // Arrange
        var account = RewardAccount.Create(Guid.NewGuid(), Guid.NewGuid());
        account.EarnPoints(100);

        // Assert
        Assert.That(account.CanRedeem(500), Is.False);
    }

    [Test]
    public void RedeemPoints_DecreasesAvailableButNotTotal()
    {
        // Arrange
        var account = RewardAccount.Create(Guid.NewGuid(), Guid.NewGuid());
        account.EarnPoints(1000);

        // Act
        account.RedeemPoints(300);

        // Assert
        Assert.That(account.AvailablePoints, Is.EqualTo(700));
        Assert.That(account.TotalEarned, Is.EqualTo(1000)); // Total doesn't decrease
    }

    [Test]
    public void UpdateTier_ChangesTheTierId()
    {
        // Arrange
        var originalTier = Guid.NewGuid();
        var newTier = Guid.NewGuid();
        var account = RewardAccount.Create(Guid.NewGuid(), originalTier);

        // Act
        account.UpdateTier(newTier);

        // Assert
        Assert.That(account.TierId, Is.EqualTo(newTier));
        Assert.That(account.UpdatedAt, Is.Not.Null);
    }
}
