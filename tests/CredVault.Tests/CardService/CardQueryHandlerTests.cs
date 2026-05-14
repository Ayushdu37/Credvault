using Moq;
using NUnit.Framework;
using CardService.Application.Queries.GetCardById;
using CardService.Application.Queries.GetCards;
using CardService.Application.Queries.GetCardUtilization;
using CardService.Domain.Entities;
using CardService.Domain.Interfaces;

namespace CredVault.Tests.CardService;

[TestFixture]
public class CardQueryHandlerTests
{
    private Mock<ICreditCardRepository> _cardRepo = null!;

    [SetUp]
    public void SetUp()
    {
        _cardRepo = new Mock<ICreditCardRepository>();
    }

    // ─── GetCardByIdQueryHandler ─────────────────────────────────────

    [Test]
    public async Task GetCardById_ValidCard_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var issuerId = Guid.NewGuid();
        var card = CreditCard.Create(
            userId, "**** **** **** 5678", "hash456",
            "Test User", 3, 2027, issuerId, 80000m, 15);

        // Populate the Issuer navigation property via reflection
        var issuer = CardIssuer.Create(issuerId, "Visa", 16, "4");
        typeof(CreditCard).GetProperty("Issuer")!.SetValue(card, issuer);

        _cardRepo.Setup(r => r.GetByIdAndUserAsync(cardId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(card);

        var handler = new GetCardByIdQueryHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new GetCardByIdQuery(userId, cardId), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.CardHolderName, Is.EqualTo("Test User"));
        Assert.That(result.Data.CreditLimit, Is.EqualTo(80000m));
        Assert.That(result.Data.IssuerName, Is.EqualTo("Visa"));
    }

    [Test]
    public async Task GetCardById_NotFound_ReturnsFailure()
    {
        // Arrange
        _cardRepo.Setup(r => r.GetByIdAndUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreditCard?)null);

        var handler = new GetCardByIdQueryHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new GetCardByIdQuery(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("not found"));
    }

    [Test]
    public async Task GetCardById_DeletedCard_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var card = CreditCard.Create(
            userId, "**** 9999", "hash999", "Deleted", 1, 2030, Guid.NewGuid(), 50000m, 1);
        card.SoftDelete();

        _cardRepo.Setup(r => r.GetByIdAndUserAsync(cardId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(card);

        var handler = new GetCardByIdQueryHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new GetCardByIdQuery(userId, cardId), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("not found"));
    }

    // ─── GetCardUtilizationQueryHandler ──────────────────────────────

    [Test]
    public async Task GetCardUtilization_MultipleCards_CalculatesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var card1 = CreditCard.Create(userId, "**** 1111", "h1", "A", 1, 2030, Guid.NewGuid(), 100000m, 1);
        card1.UpdateOutstandingBalance(20000m);
        var card2 = CreditCard.Create(userId, "**** 2222", "h2", "B", 2, 2030, Guid.NewGuid(), 100000m, 1);
        card2.UpdateOutstandingBalance(30000m);

        _cardRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CreditCard> { card1, card2 });

        var handler = new GetCardUtilizationQueryHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new GetCardUtilizationQuery(userId), CancellationToken.None);

        // Assert — total limit 200000, total balance 50000, utilization 25%
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data!.TotalCards, Is.EqualTo(2));
        Assert.That(result.Data.TotalCreditLimit, Is.EqualTo(200000m));
        Assert.That(result.Data.TotalOutstandingBalance, Is.EqualTo(50000m));
        Assert.That(result.Data.TotalAvailableCredit, Is.EqualTo(150000m));
        Assert.That(result.Data.UtilizationPercentage, Is.EqualTo(25m));
    }

    [Test]
    public async Task GetCardUtilization_ExcludesDeletedCards()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var active = CreditCard.Create(userId, "**** 1111", "h1", "A", 1, 2030, Guid.NewGuid(), 100000m, 1);
        active.UpdateOutstandingBalance(10000m);
        var deleted = CreditCard.Create(userId, "**** 2222", "h2", "B", 2, 2030, Guid.NewGuid(), 50000m, 1);
        deleted.UpdateOutstandingBalance(50000m);
        deleted.SoftDelete();

        _cardRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CreditCard> { active, deleted });

        var handler = new GetCardUtilizationQueryHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new GetCardUtilizationQuery(userId), CancellationToken.None);

        // Assert
        Assert.That(result.Data!.TotalCards, Is.EqualTo(1));
        Assert.That(result.Data.TotalCreditLimit, Is.EqualTo(100000m));
    }

    [Test]
    public async Task GetCardUtilization_NoCards_ReturnsZero()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _cardRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CreditCard>());

        var handler = new GetCardUtilizationQueryHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new GetCardUtilizationQuery(userId), CancellationToken.None);

        // Assert
        Assert.That(result.Data!.UtilizationPercentage, Is.EqualTo(0));
        Assert.That(result.Data.TotalCards, Is.EqualTo(0));
    }
}
