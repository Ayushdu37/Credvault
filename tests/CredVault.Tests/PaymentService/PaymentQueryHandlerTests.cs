using Moq;
using NUnit.Framework;
using PaymentService.Application.Queries.GetPaymentById;
using PaymentService.Application.Queries.GetPayments;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using CredVault.Shared.Contracts.Enums;

namespace CredVault.Tests.PaymentService;

[TestFixture]
public class PaymentQueryHandlerTests
{
    private Mock<IPaymentRepository> _paymentRepo = null!;

    [SetUp]
    public void SetUp()
    {
        _paymentRepo = new Mock<IPaymentRepository>();
    }

    // ─── GetPaymentById ──────────────────────────────────────────────

    [Test]
    public async Task GetPaymentById_Found_ReturnsPaymentResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var payment = Payment.Create(userId, Guid.NewGuid(), Guid.NewGuid(), 5000m, "CreditCard", "TXN-1");
        payment.MarkCompleted();

        _paymentRepo.Setup(r => r.GetByIdAndUserAsync(paymentId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var handler = new GetPaymentByIdQueryHandler(_paymentRepo.Object);
        var query = new GetPaymentByIdQuery(userId, paymentId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Amount, Is.EqualTo(5000m));
        Assert.That(result.Data.Status, Is.EqualTo(PaymentStatus.Completed));
    }

    [Test]
    public async Task GetPaymentById_NotFound_ReturnsFailure()
    {
        // Arrange
        _paymentRepo.Setup(r => r.GetByIdAndUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);

        var handler = new GetPaymentByIdQueryHandler(_paymentRepo.Object);
        var query = new GetPaymentByIdQuery(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("not found"));
    }

    // ─── GetPayments (paginated) ─────────────────────────────────────

    [Test]
    public async Task GetPayments_ReturnsCorrectPageSize()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var payments = Enumerable.Range(1, 5)
            .Select(i => Payment.Create(userId, Guid.NewGuid(), Guid.NewGuid(), i * 100m, "UPI"))
            .ToList();

        _paymentRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payments);

        var handler = new GetPaymentsQueryHandler(_paymentRepo.Object);
        var query = new GetPaymentsQuery(userId, Page: 1, PageSize: 3);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data!.Items, Has.Count.EqualTo(3));
        Assert.That(result.Data.TotalCount, Is.EqualTo(5));
    }

    [Test]
    public async Task GetPayments_EmptyList_ReturnsEmptyResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _paymentRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Payment>());

        var handler = new GetPaymentsQueryHandler(_paymentRepo.Object);
        var query = new GetPaymentsQuery(userId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data!.Items, Is.Empty);
        Assert.That(result.Data.TotalCount, Is.EqualTo(0));
    }
}
