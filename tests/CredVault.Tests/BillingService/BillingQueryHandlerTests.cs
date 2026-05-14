using Moq;
using NUnit.Framework;
using BillingService.Application.Queries.GetBillsByCard;
using BillingService.Application.Queries.GetBills;
using BillingService.Domain.Entities;
using BillingService.Domain.Interfaces;
using CredVault.Shared.Contracts.Enums;

namespace CredVault.Tests.BillingService;

[TestFixture]
public class BillingQueryHandlerTests
{
    private Mock<IBillRepository> _billRepo = null!;

    [SetUp]
    public void SetUp()
    {
        _billRepo = new Mock<IBillRepository>();
    }

    // ─── GetBillsByCardQueryHandler ──────────────────────────────────

    [Test]
    public async Task GetBillsByCard_HasBills_ReturnsMappedList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var bill1 = Bill.Create(userId, cardId, 10000m, 500m, DateTime.UtcNow.AddDays(15), "March 2026");
        var bill2 = Bill.Create(userId, cardId, 8000m, 400m, DateTime.UtcNow.AddDays(30), "April 2026");

        _billRepo.Setup(r => r.GetByCardIdAsync(cardId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Bill> { bill1, bill2 });

        var handler = new GetBillsByCardQueryHandler(_billRepo.Object);

        // Act
        var result = await handler.Handle(new GetBillsByCardQuery(userId, cardId), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetBillsByCard_NoBills_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        _billRepo.Setup(r => r.GetByCardIdAsync(cardId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Bill>());

        var handler = new GetBillsByCardQueryHandler(_billRepo.Object);

        // Act
        var result = await handler.Handle(new GetBillsByCardQuery(userId, cardId), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Empty);
    }

    [Test]
    public async Task GetBillsByCard_PaidBill_ReturnsCorrectStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var bill = Bill.Create(userId, cardId, 5000m, 250m, DateTime.UtcNow.AddDays(15), "March 2026");
        bill.ApplyPayment(5000m);

        _billRepo.Setup(r => r.GetByCardIdAsync(cardId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Bill> { bill });

        var handler = new GetBillsByCardQueryHandler(_billRepo.Object);

        // Act
        var result = await handler.Handle(new GetBillsByCardQuery(userId, cardId), CancellationToken.None);

        // Assert
        Assert.That(result.Data![0].Status, Is.EqualTo(BillStatus.Paid));
    }

    // ─── GetBillsQueryHandler (paginated) ────────────────────────────

    [Test]
    public async Task GetBills_ReturnsCorrectPageSize()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bills = Enumerable.Range(1, 5)
            .Select(i => Bill.Create(userId, Guid.NewGuid(), i * 1000m, i * 50m,
                DateTime.UtcNow.AddDays(30), $"Month {i}"))
            .ToList();

        _billRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bills);

        var handler = new GetBillsQueryHandler(_billRepo.Object);
        var query = new GetBillsQuery(userId, Page: 1, PageSize: 3);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data!.Items, Has.Count.EqualTo(3));
        Assert.That(result.Data.TotalCount, Is.EqualTo(5));
    }

    [Test]
    public async Task GetBills_EmptyList_ReturnsEmptyResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _billRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Bill>());

        var handler = new GetBillsQueryHandler(_billRepo.Object);

        // Act
        var result = await handler.Handle(new GetBillsQuery(userId), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data!.Items, Is.Empty);
    }
}
