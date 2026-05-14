using Moq;
using NUnit.Framework;
using PaymentService.Application.Commands.AddPaymentMethod;
using PaymentService.Application.Commands.RemovePaymentMethod;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;

namespace CredVault.Tests.PaymentService;

[TestFixture]
public class PaymentMethodCommandHandlerTests
{
    private Mock<ISavedPaymentMethodRepository> _methodRepo = null!;

    [SetUp]
    public void SetUp()
    {
        _methodRepo = new Mock<ISavedPaymentMethodRepository>();
    }

    // ─── AddPaymentMethod ────────────────────────────────────────────

    [Test]
    public async Task AddPaymentMethod_FirstMethod_SetsAsDefault()
    {
        // Arrange
        _methodRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SavedPaymentMethod>()); // empty = first method
        _methodRepo.Setup(r => r.AddAsync(It.IsAny<SavedPaymentMethod>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new AddPaymentMethodCommandHandler(_methodRepo.Object);
        var command = new AddPaymentMethodCommand(Guid.NewGuid(), "UPI", "My UPI", "upi@bank");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.EqualTo(Guid.Empty));
        _methodRepo.Verify(r => r.AddAsync(
            It.Is<SavedPaymentMethod>(m => m.IsDefault == true),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task AddPaymentMethod_SubsequentMethod_NotDefault()
    {
        // Arrange — user already has one method
        var existing = SavedPaymentMethod.Create(Guid.NewGuid(), "UPI", "Old UPI", "old@bank");
        _methodRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SavedPaymentMethod> { existing });
        _methodRepo.Setup(r => r.AddAsync(It.IsAny<SavedPaymentMethod>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new AddPaymentMethodCommandHandler(_methodRepo.Object);
        var command = new AddPaymentMethodCommand(Guid.NewGuid(), "NetBanking", "HDFC", "hdfc-details");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        _methodRepo.Verify(r => r.AddAsync(
            It.Is<SavedPaymentMethod>(m => m.IsDefault == false),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── RemovePaymentMethod ─────────────────────────────────────────

    [Test]
    public async Task RemovePaymentMethod_Exists_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var method = SavedPaymentMethod.Create(userId, "UPI", "My UPI", "upi@bank");

        _methodRepo.Setup(r => r.GetByIdAndUserAsync(methodId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(method);
        _methodRepo.Setup(r => r.DeleteAsync(method, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new RemovePaymentMethodCommandHandler(_methodRepo.Object);
        var command = new RemovePaymentMethodCommand(userId, methodId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        _methodRepo.Verify(r => r.DeleteAsync(method, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task RemovePaymentMethod_NotFound_ReturnsFailure()
    {
        // Arrange
        _methodRepo.Setup(r => r.GetByIdAndUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SavedPaymentMethod?)null);

        var handler = new RemovePaymentMethodCommandHandler(_methodRepo.Object);
        var command = new RemovePaymentMethodCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("not found"));
    }
}
