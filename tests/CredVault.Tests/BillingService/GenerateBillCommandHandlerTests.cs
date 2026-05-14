using Moq;
using NUnit.Framework;
using BillingService.Application.Commands.GenerateBill;
using BillingService.Domain.Entities;
using BillingService.Domain.Interfaces;
using BillingService.Application.Abstractions;

namespace CredVault.Tests.BillingService;

[TestFixture]
public class GenerateBillCommandHandlerTests
{
    private Mock<IBillRepository> _billRepo = null!;
    private Mock<IEventPublisher> _eventPublisher = null!;
    private GenerateBillCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _billRepo = new Mock<IBillRepository>();
        _eventPublisher = new Mock<IEventPublisher>();
        _handler = new GenerateBillCommandHandler(_billRepo.Object, _eventPublisher.Object);
    }

    [Test]
    public async Task GenerateBill_NoDuplicate_CreatesBillAndPublishesEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        _billRepo.Setup(r => r.GetByCardAndMonthAsync(cardId, "April 2026", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bill?)null);
        _billRepo.Setup(r => r.AddAsync(It.IsAny<Bill>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _eventPublisher.Setup(e => e.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new GenerateBillCommand(
            userId, cardId, 15000m, 750m,
            DateTime.UtcNow.AddDays(30), "April 2026");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.EqualTo(Guid.Empty));
        _billRepo.Verify(r => r.AddAsync(It.IsAny<Bill>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GenerateBill_DuplicateMonth_ReturnsFailure()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var existingBill = Bill.Create(Guid.NewGuid(), cardId, 10000m, 500m,
            DateTime.UtcNow.AddDays(30), "April 2026");
        _billRepo.Setup(r => r.GetByCardAndMonthAsync(cardId, "April 2026", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBill);

        var command = new GenerateBillCommand(
            Guid.NewGuid(), cardId, 15000m, 750m,
            DateTime.UtcNow.AddDays(30), "April 2026");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("already exists"));
    }

    // ─── INTENTIONALLY FAILING TESTS ─────────────────────────────────

    [Test]
    public async Task GenerateBill_DuplicateMonth_ShouldFail_ButExpectsSuccess()
    {
        // Arrange — bill already exists for April 2026
        var cardId = Guid.NewGuid();
        var existingBill = Bill.Create(Guid.NewGuid(), cardId, 10000m, 500m,
            DateTime.UtcNow.AddDays(30), "April 2026");
        _billRepo.Setup(r => r.GetByCardAndMonthAsync(cardId, "April 2026", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBill);

        var command = new GenerateBillCommand(
            Guid.NewGuid(), cardId, 20000m, 1000m,
            DateTime.UtcNow.AddDays(30), "April 2026");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert — WRONG: expecting success for a duplicate billing month → WILL FAIL
        Assert.That(result.Success, Is.True, "BUG: Duplicate bill for same card+month should NOT succeed");
        Assert.That(result.Data, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void NewBill_ShouldBePending_ButExpectsPaid()
    {
        // Arrange & Act — newly created bill with no payments
        var bill = Bill.Create(Guid.NewGuid(), Guid.NewGuid(), 15000m, 750m,
            DateTime.UtcNow.AddDays(30), "May 2026");

        // Assert — WRONG: expecting "Paid" status on a brand-new bill → WILL FAIL
        Assert.That(bill.Status, Is.EqualTo("Paid"), "BUG: A new bill should be 'Pending', not 'Paid'");
        Assert.That(bill.AmountPaid, Is.EqualTo(15000m));
    }
}
