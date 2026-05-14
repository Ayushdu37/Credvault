using NUnit.Framework;
using PaymentService.Domain.Entities;

namespace CredVault.Tests.PaymentService;

[TestFixture]
public class PaymentEntityTests
{
    [Test]
    public void Create_SetsStatusToProcessing()
    {
        // Arrange & Act
        var payment = Payment.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            1500m, "CreditCard", "TXN-001");

        // Assert
        Assert.That(payment.Status, Is.EqualTo("Processing"));
        Assert.That(payment.IsProcessing, Is.True);
        Assert.That(payment.IsCompleted, Is.False);
        Assert.That(payment.Amount, Is.EqualTo(1500m));
        Assert.That(payment.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Create_AssignsAllProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var billId = Guid.NewGuid();
        var cardId = Guid.NewGuid();

        // Act
        var payment = Payment.Create(userId, billId, cardId, 2000m, "UPI", "ref-123");

        // Assert
        Assert.That(payment.UserId, Is.EqualTo(userId));
        Assert.That(payment.BillId, Is.EqualTo(billId));
        Assert.That(payment.CardId, Is.EqualTo(cardId));
        Assert.That(payment.PaymentMethod, Is.EqualTo("UPI"));
        Assert.That(payment.TransactionReference, Is.EqualTo("ref-123"));
    }

    [Test]
    public void MarkCompleted_SetsStatusAndCompletedAt()
    {
        // Arrange
        var payment = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 500m, "UPI");

        // Act
        payment.MarkCompleted();

        // Assert
        Assert.That(payment.Status, Is.EqualTo("Completed"));
        Assert.That(payment.IsCompleted, Is.True);
        Assert.That(payment.IsProcessing, Is.False);
        Assert.That(payment.CompletedAt, Is.Not.Null);
    }

    [Test]
    public void MarkFailed_SetsStatusAndFailureReason()
    {
        // Arrange
        var payment = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 500m, "UPI");

        // Act
        payment.MarkFailed("Gateway declined");

        // Assert
        Assert.That(payment.Status, Is.EqualTo("Failed"));
        Assert.That(payment.FailureReason, Is.EqualTo("Gateway declined"));
    }

    [Test]
    public void MarkRefunded_SetsStatusToRefunded()
    {
        // Arrange
        var payment = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 500m, "UPI");
        payment.MarkCompleted();

        // Act
        payment.MarkRefunded();

        // Assert
        Assert.That(payment.Status, Is.EqualTo("Refunded"));
    }
}
