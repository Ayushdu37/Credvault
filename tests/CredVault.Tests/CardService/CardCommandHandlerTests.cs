using Moq;
using NUnit.Framework;
using CardService.Application.Commands.AddCard;
using CardService.Application.Commands.RemoveCard;
using CardService.Application.Commands.VerifyCard;
using CardService.Application.Commands.UpdateCardLimit;
using CardService.Application.Commands.SetDefaultCard;
using CardService.Application.Abstractions;
using CardService.Domain.Entities;
using CardService.Domain.Interfaces;
using CardIssuerEnum = CredVault.Shared.Contracts.Enums.CardIssuer;

namespace CredVault.Tests.CardService;

[TestFixture]
public class CardCommandHandlerTests
{
    private Mock<ICreditCardRepository> _cardRepo = null!;
    private Mock<ICardIssuerRepository> _issuerRepo = null!;
    private Mock<ICardHasher> _hasher = null!;
    private Mock<IEventPublisher> _events = null!;

    [SetUp]
    public void SetUp()
    {
        _cardRepo = new Mock<ICreditCardRepository>();
        _issuerRepo = new Mock<ICardIssuerRepository>();
        _hasher = new Mock<ICardHasher>();
        _events = new Mock<IEventPublisher>();
    }

    // ─── AddCardCommandHandler ───────────────────────────────────────

    [Test]
    public async Task AddCard_ValidCard_ReturnsSuccessAndPublishesEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var issuerId = Guid.NewGuid();
        var issuer = CardIssuer.Create(issuerId, "Visa", 16, "4");

        _hasher.Setup(h => h.HashCardNumber("4111111111111234")).Returns("hash-visa");
        _hasher.Setup(h => h.MaskCardNumber("4111111111111234")).Returns("**** **** **** 1234");
        _cardRepo.Setup(r => r.ExistsByHashAsync(userId, "hash-visa", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _issuerRepo.Setup(r => r.GetByNameAsync("Visa", It.IsAny<CancellationToken>())).ReturnsAsync(issuer);
        _cardRepo.Setup(r => r.AddAsync(It.IsAny<CreditCard>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _events.Setup(e => e.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new AddCardCommandHandler(_cardRepo.Object, _issuerRepo.Object, _hasher.Object, _events.Object);
        var command = new AddCardCommand(userId, "4111111111111234", "John", 12, 2028,
            CardIssuerEnum.Visa, 100000m, 1, "My Visa");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.EqualTo(Guid.Empty));
        _cardRepo.Verify(r => r.AddAsync(It.IsAny<CreditCard>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task AddCard_DuplicateCard_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _hasher.Setup(h => h.HashCardNumber(It.IsAny<string>())).Returns("hash-dup");
        _cardRepo.Setup(r => r.ExistsByHashAsync(userId, "hash-dup", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new AddCardCommandHandler(_cardRepo.Object, _issuerRepo.Object, _hasher.Object, _events.Object);
        var command = new AddCardCommand(userId, "4111111111111234", "John", 12, 2028,
            CardIssuerEnum.Visa, 100000m, 1, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("already been added"));
    }

    [Test]
    public async Task AddCard_UnknownIssuer_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _hasher.Setup(h => h.HashCardNumber(It.IsAny<string>())).Returns("hash-unknown");
        _cardRepo.Setup(r => r.ExistsByHashAsync(userId, "hash-unknown", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _issuerRepo.Setup(r => r.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CardIssuer?)null);

        var handler = new AddCardCommandHandler(_cardRepo.Object, _issuerRepo.Object, _hasher.Object, _events.Object);
        var command = new AddCardCommand(userId, "4111111111111234", "John", 12, 2028,
            CardIssuerEnum.Visa, 100000m, 1, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Unknown card issuer"));
    }

    // ─── RemoveCardCommandHandler ────────────────────────────────────

    [Test]
    public async Task RemoveCard_ExistingCard_SoftDeletesAndReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var card = CreditCard.Create(userId, "**** 1234", "h", "Jane", 6, 2029, Guid.NewGuid(), 50000m, 1);

        _cardRepo.Setup(r => r.GetByIdAndUserAsync(cardId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _cardRepo.Setup(r => r.UpdateAsync(card, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new RemoveCardCommandHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new RemoveCardCommand(userId, cardId), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(card.IsDeleted, Is.True);
    }

    [Test]
    public async Task RemoveCard_AlreadyDeleted_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var card = CreditCard.Create(userId, "**** 1234", "h", "Jane", 6, 2029, Guid.NewGuid(), 50000m, 1);
        card.SoftDelete();

        _cardRepo.Setup(r => r.GetByIdAndUserAsync(cardId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(card);

        var handler = new RemoveCardCommandHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new RemoveCardCommand(userId, cardId), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("already been removed"));
    }

    [Test]
    public async Task RemoveCard_NotFound_ReturnsFailure()
    {
        // Arrange
        _cardRepo.Setup(r => r.GetByIdAndUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreditCard?)null);

        var handler = new RemoveCardCommandHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new RemoveCardCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("not found"));
    }

    // ─── SetDefaultCardCommandHandler ────────────────────────────────

    [Test]
    public async Task SetDefault_UnsetsOldDefault_SetsNewDefault()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var oldDefault = CreditCard.Create(userId, "**** 1111", "h1", "A", 1, 2030, Guid.NewGuid(), 100000m, 1);
        oldDefault.SetAsDefault();
        var newCard = CreditCard.Create(userId, "**** 2222", "h2", "B", 2, 2030, Guid.NewGuid(), 100000m, 1);

        _cardRepo.Setup(r => r.GetByIdAndUserAsync(cardId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(newCard);
        _cardRepo.Setup(r => r.GetDefaultByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(oldDefault);
        _cardRepo.Setup(r => r.UpdateAsync(It.IsAny<CreditCard>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new SetDefaultCardCommandHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new SetDefaultCardCommand(userId, cardId), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(newCard.IsDefault, Is.True);
        Assert.That(oldDefault.IsDefault, Is.False);
    }

    [Test]
    public async Task SetDefault_DeletedCard_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var card = CreditCard.Create(userId, "**** 1234", "h", "X", 1, 2030, Guid.NewGuid(), 50000m, 1);
        card.SoftDelete();

        _cardRepo.Setup(r => r.GetByIdAndUserAsync(cardId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(card);

        var handler = new SetDefaultCardCommandHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new SetDefaultCardCommand(userId, cardId), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
    }

    // ─── VerifyCardCommandHandler ────────────────────────────────────

    [Test]
    public async Task VerifyCard_UnverifiedCard_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var card = CreditCard.Create(userId, "**** 1234", "h", "X", 1, 2030, Guid.NewGuid(), 50000m, 1);

        _cardRepo.Setup(r => r.GetByIdAndUserAsync(cardId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _cardRepo.Setup(r => r.UpdateAsync(card, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new VerifyCardCommandHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new VerifyCardCommand(userId, cardId), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(card.IsVerified, Is.True);
    }

    [Test]
    public async Task VerifyCard_AlreadyVerified_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var card = CreditCard.Create(userId, "**** 1234", "h", "X", 1, 2030, Guid.NewGuid(), 50000m, 1);
        card.Verify();

        _cardRepo.Setup(r => r.GetByIdAndUserAsync(cardId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(card);

        var handler = new VerifyCardCommandHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new VerifyCardCommand(userId, cardId), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("already verified"));
    }

    // ─── UpdateCardLimitCommandHandler ───────────────────────────────

    [Test]
    public async Task UpdateCardLimit_ValidLimit_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var card = CreditCard.Create(userId, "**** 1234", "h", "X", 1, 2030, Guid.NewGuid(), 50000m, 1);

        _cardRepo.Setup(r => r.GetByIdAndUserAsync(cardId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _cardRepo.Setup(r => r.UpdateAsync(card, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new UpdateCardLimitCommandHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new UpdateCardLimitCommand(userId, cardId, 200000m), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(card.CreditLimit, Is.EqualTo(200000m));
    }

    [TestCase(0)]
    [TestCase(-50000)]
    public async Task UpdateCardLimit_InvalidLimit_ReturnsFailure(decimal invalidLimit)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var card = CreditCard.Create(userId, "**** 1234", "h", "X", 1, 2030, Guid.NewGuid(), 50000m, 1);

        _cardRepo.Setup(r => r.GetByIdAndUserAsync(cardId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(card);

        var handler = new UpdateCardLimitCommandHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new UpdateCardLimitCommand(userId, cardId, invalidLimit), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("greater than zero"));
    }

    // ─── INTENTIONALLY FAILING TESTS ─────────────────────────────────

    [Test]
    public async Task AddCard_DuplicateCard_ShouldFail_ButExpectsSuccess()
    {
        // Arrange — card hash already exists for this user
        var userId = Guid.NewGuid();
        _hasher.Setup(h => h.HashCardNumber(It.IsAny<string>())).Returns("hash-dup");
        _cardRepo.Setup(r => r.ExistsByHashAsync(userId, "hash-dup", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new AddCardCommandHandler(_cardRepo.Object, _issuerRepo.Object, _hasher.Object, _events.Object);
        var command = new AddCardCommand(userId, "4111111111111234", "John", 12, 2028,
            CardIssuerEnum.Visa, 100000m, 1, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert — WRONG: expecting success for a duplicate card → WILL FAIL
        Assert.That(result.Success, Is.True, "BUG: Duplicate card should NOT be added successfully");
        Assert.That(result.Data, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task UpdateCardLimit_ZeroLimit_ShouldFail_ButExpectsSuccess()
    {
        // Arrange — limit is set to 0 (invalid)
        var userId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var card = CreditCard.Create(userId, "**** 1234", "h", "X", 1, 2030, Guid.NewGuid(), 50000m, 1);

        _cardRepo.Setup(r => r.GetByIdAndUserAsync(cardId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(card);

        var handler = new UpdateCardLimitCommandHandler(_cardRepo.Object);

        // Act
        var result = await handler.Handle(new UpdateCardLimitCommand(userId, cardId, 0m), CancellationToken.None);

        // Assert — WRONG: expecting success for zero credit limit → WILL FAIL
        Assert.That(result.Success, Is.True, "BUG: Zero credit limit should NOT be accepted");
        Assert.That(card.CreditLimit, Is.EqualTo(0m));
    }
}
