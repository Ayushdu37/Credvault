using Moq;
using NUnit.Framework;
using PaymentService.Application.Commands.MakePayment;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using PaymentService.Application.Abstractions;
using CredVault.Shared.Contracts.Payment.Events;
using Microsoft.Extensions.Logging;

namespace CredVault.Tests.PaymentService;

[TestFixture]
public class MakePaymentCommandHandlerTests
{
    private Mock<IPaymentRepository> _paymentRepo = null!;
    private Mock<IEventPublisher> _eventPublisher = null!;
    private Mock<ILogger<MakePaymentCommandHandler>> _logger = null!;
    private MakePaymentCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _paymentRepo = new Mock<IPaymentRepository>();
        _eventPublisher = new Mock<IEventPublisher>();
        _logger = new Mock<ILogger<MakePaymentCommandHandler>>();
        _handler = new MakePaymentCommandHandler(
            _paymentRepo.Object, _eventPublisher.Object, _logger.Object);
    }

    [Test]
    public async Task Handle_ValidPayment_ReturnsSuccessAndPublishesCompletedEvent()
    {
        // Arrange
        var command = new MakePaymentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            1500m, "CreditCard", "TXN-001");

        _paymentRepo.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _paymentRepo.Setup(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _eventPublisher.Setup(e => e.PublishAsync(It.IsAny<PaymentCompletedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.Message, Does.Contain("completed"));
        _paymentRepo.Verify(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
        _paymentRepo.Verify(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisher.Verify(e => e.PublishAsync(It.IsAny<PaymentCompletedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_GatewayDecline_ReturnsFailureAndPublishesFailedEvent()
    {
        // Arrange — amount 99999 triggers simulated gateway failure
        var command = new MakePaymentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            99999m, "CreditCard", null);

        _paymentRepo.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _paymentRepo.Setup(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _eventPublisher.Setup(e => e.PublishAsync(It.IsAny<PaymentFailedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Payment failed"));
        _eventPublisher.Verify(e => e.PublishAsync(It.IsAny<PaymentFailedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_CreditCardMethod_PublishesEventWithCorrectMethod()
    {
        // Arrange
        var command = new MakePaymentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            500m, "CreditCard", null);

        PaymentCompletedEvent? captured = null;
        _paymentRepo.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _paymentRepo.Setup(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _eventPublisher.Setup(e => e.PublishAsync(It.IsAny<PaymentCompletedEvent>(), It.IsAny<CancellationToken>()))
            .Callback<PaymentCompletedEvent, CancellationToken>((evt, _) => captured = evt)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.PaymentMethod, Is.EqualTo("CreditCard"));
        Assert.That(captured.Amount, Is.EqualTo(500m));
    }

    [Test]
    public async Task Handle_UpiMethod_PublishesEventWithUpiMethod()
    {
        // Arrange
        var command = new MakePaymentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            750m, "UPI", "upi-ref-123");

        PaymentCompletedEvent? captured = null;
        _paymentRepo.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _paymentRepo.Setup(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _eventPublisher.Setup(e => e.PublishAsync(It.IsAny<PaymentCompletedEvent>(), It.IsAny<CancellationToken>()))
            .Callback<PaymentCompletedEvent, CancellationToken>((evt, _) => captured = evt)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.PaymentMethod, Is.EqualTo("UPI"));
    }

    // ─── INTENTIONALLY FAILING TESTS ─────────────────────────────────

    [Test]
    public async Task Handle_GatewayDecline_ShouldFail_ButExpectsSuccess()
    {
        // Arrange — amount 99999 triggers simulated gateway decline
        var command = new MakePaymentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            99999m, "CreditCard", null);

        _paymentRepo.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _paymentRepo.Setup(r => r.UpdateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _eventPublisher.Setup(e => e.PublishAsync(It.IsAny<PaymentFailedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert — WRONG: expecting success for a gateway-declined payment → WILL FAIL
        Assert.That(result.Success, Is.True, "BUG: Gateway-declined payment should NOT return success");
        Assert.That(result.Message, Does.Contain("completed"));
    }

    [Test]
    public void NewPayment_ShouldBeProcessing_ButExpectsCompleted()
    {
        // Arrange & Act — create a brand-new payment (no processing yet)
        var payment = Payment.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            2500m, "NetBanking", "NB-REF-001");

        // Assert — WRONG: expecting "Completed" on a freshly created payment → WILL FAIL
        Assert.That(payment.Status, Is.EqualTo("Completed"), "BUG: New payment should be 'Processing', not 'Completed'");
        Assert.That(payment.IsCompleted, Is.True);
    }
}
