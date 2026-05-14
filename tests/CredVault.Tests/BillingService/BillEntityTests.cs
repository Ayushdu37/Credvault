using NUnit.Framework;
using BillingService.Domain.Entities;

namespace CredVault.Tests.BillingService;

[TestFixture]
public class BillEntityTests
{
    [Test]
    public void Create_SetsCorrectValues()
    {
        // Arrange & Act
        var bill = Bill.Create(Guid.NewGuid(), Guid.NewGuid(), 10000m, 500m, DateTime.UtcNow.AddDays(30), "April 2026");

        // Assert
        Assert.That(bill.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(bill.TotalAmount, Is.EqualTo(10000m));
        Assert.That(bill.MinimumDue, Is.EqualTo(500m));
        Assert.That(bill.AmountPaid, Is.EqualTo(0m));
        Assert.That(bill.Status, Is.EqualTo("Pending"));
    }

    [Test]
    public void Remaining_CalculatedCorrectly()
    {
        // Arrange
        var bill = Bill.Create(Guid.NewGuid(), Guid.NewGuid(), 10000m, 500m, DateTime.UtcNow.AddDays(30), "April 2026");

        // Act
        bill.ApplyPayment(3000m);

        // Assert
        Assert.That(bill.Remaining, Is.EqualTo(7000m));
    }

    [Test]
    public void ApplyPayment_FullPayment_MarksAsPaid()
    {
        // Arrange
        var bill = Bill.Create(Guid.NewGuid(), Guid.NewGuid(), 5000m, 250m, DateTime.UtcNow.AddDays(30), "April 2026");

        // Act
        bill.ApplyPayment(5000m);

        // Assert
        Assert.That(bill.Status, Is.EqualTo("Paid"));
        Assert.That(bill.AmountPaid, Is.EqualTo(5000m));
        Assert.That(bill.Remaining, Is.EqualTo(0m));
    }

    [Test]
    public void ApplyPayment_PartialPayment_MarksAsPartiallyPaid()
    {
        // Arrange
        var bill = Bill.Create(Guid.NewGuid(), Guid.NewGuid(), 10000m, 500m, DateTime.UtcNow.AddDays(30), "April 2026");

        // Act
        bill.ApplyPayment(2000m);

        // Assert
        Assert.That(bill.Status, Is.EqualTo("PartiallyPaid"));
        Assert.That(bill.AmountPaid, Is.EqualTo(2000m));
    }

    [Test]
    public void ApplyPayment_MultiplePartialPayments_AccumulatesCorrectly()
    {
        // Arrange
        var bill = Bill.Create(Guid.NewGuid(), Guid.NewGuid(), 10000m, 500m, DateTime.UtcNow.AddDays(30), "April 2026");

        // Act
        bill.ApplyPayment(3000m);
        bill.ApplyPayment(7000m);

        // Assert
        Assert.That(bill.Status, Is.EqualTo("Paid"));
        Assert.That(bill.AmountPaid, Is.EqualTo(10000m));
    }

    [Test]
    public void MarkOverdue_UnpaidBill_SetsStatusToOverdue()
    {
        // Arrange
        var bill = Bill.Create(Guid.NewGuid(), Guid.NewGuid(), 5000m, 250m, DateTime.UtcNow.AddDays(-1), "March 2026");

        // Act
        bill.MarkOverdue();

        // Assert
        Assert.That(bill.Status, Is.EqualTo("Overdue"));
    }

    [Test]
    public void MarkOverdue_PaidBill_DoesNotChangeStatus()
    {
        // Arrange
        var bill = Bill.Create(Guid.NewGuid(), Guid.NewGuid(), 5000m, 250m, DateTime.UtcNow.AddDays(-1), "March 2026");
        bill.ApplyPayment(5000m);

        // Act
        bill.MarkOverdue();

        // Assert
        Assert.That(bill.Status, Is.EqualTo("Paid"));
    }

    [Test]
    public void IsOverdue_PastDueAndUnpaid_ReturnsTrue()
    {
        // Arrange
        var bill = Bill.Create(Guid.NewGuid(), Guid.NewGuid(), 5000m, 250m, DateTime.UtcNow.AddDays(-1), "March 2026");

        // Assert
        Assert.That(bill.IsOverdue, Is.True);
    }

    [Test]
    public void IsOverdue_PastDueButPaid_ReturnsFalse()
    {
        // Arrange
        var bill = Bill.Create(Guid.NewGuid(), Guid.NewGuid(), 5000m, 250m, DateTime.UtcNow.AddDays(-1), "March 2026");
        bill.ApplyPayment(5000m);

        // Assert
        Assert.That(bill.IsOverdue, Is.False);
    }

    [Test]
    public void DueDate_IsSetCorrectly()
    {
        // Arrange
        var dueDate = new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var bill = Bill.Create(Guid.NewGuid(), Guid.NewGuid(), 5000m, 250m, dueDate, "May 2026");

        // Assert
        Assert.That(bill.DueDate, Is.EqualTo(dueDate));
    }
}
