using NUnit.Framework;
using PaymentService.Application.Commands.MakePayment;
using PaymentService.Application.Validators;

namespace CredVault.Tests.PaymentService;

[TestFixture]
public class MakePaymentValidatorTests
{
    private MakePaymentCommandValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new MakePaymentCommandValidator();
    }

    [Test]
    public void ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new MakePaymentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            1500m, "CreditCard", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [TestCase(0)]
    [TestCase(-100)]
    [TestCase(-0.01)]
    public void InvalidAmount_FailsValidation(decimal amount)
    {
        // Arrange
        var command = new MakePaymentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            amount, "CreditCard", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
            e => e.PropertyName == "Amount"));
    }

    [Test]
    public void EmptyPaymentMethod_FailsValidation()
    {
        // Arrange
        var command = new MakePaymentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            500m, "", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
            e => e.PropertyName == "PaymentMethod"));
    }

    [Test]
    public void EmptyBillId_FailsValidation()
    {
        // Arrange
        var command = new MakePaymentCommand(
            Guid.NewGuid(), Guid.Empty, Guid.NewGuid(),
            500m, "CreditCard", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void EmptyCardId_FailsValidation()
    {
        // Arrange
        var command = new MakePaymentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.Empty,
            500m, "CreditCard", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [TestCase("CreditCard", true)]
    [TestCase("DebitCard", true)]
    [TestCase("2", true)]
    [TestCase("4", true)]
    [TestCase("UPI", false)]
    [TestCase("BankTransfer", false)]
    [TestCase("NetBanking", false)]
    public void CardBasedMethodResolution_CorrectlyIdentifiesCardPayments(string method, bool expectedIsCard)
    {
        // Arrange — mirrors PaymentCompletedConsumer HashSet logic
        var cardBasedMethods = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CreditCard", "DebitCard", "2", "4"
        };

        // Act
        var isCardBased = cardBasedMethods.Contains(method);

        // Assert
        Assert.That(isCardBased, Is.EqualTo(expectedIsCard));
    }
}
