using NUnit.Framework;
using PaymentService.Domain.Entities;

namespace CredVault.Tests.PaymentService;

[TestFixture]
public class SavedPaymentMethodEntityTests
{
    [Test]
    public void Create_SetsDefaultValues()
    {
        // Arrange & Act
        var method = SavedPaymentMethod.Create(
            Guid.NewGuid(), "UPI", "My UPI", "upi@bank");

        // Assert
        Assert.That(method.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(method.MethodType, Is.EqualTo("UPI"));
        Assert.That(method.DisplayName, Is.EqualTo("My UPI"));
        Assert.That(method.Details, Is.EqualTo("upi@bank"));
        Assert.That(method.IsDefault, Is.False);
    }

    [Test]
    public void SetAsDefault_SetsIsDefaultToTrue()
    {
        // Arrange
        var method = SavedPaymentMethod.Create(
            Guid.NewGuid(), "UPI", "My UPI", "upi@bank");

        // Act
        method.SetAsDefault();

        // Assert
        Assert.That(method.IsDefault, Is.True);
    }

    [Test]
    public void UnsetDefault_SetsIsDefaultToFalse()
    {
        // Arrange
        var method = SavedPaymentMethod.Create(
            Guid.NewGuid(), "UPI", "My UPI", "upi@bank");
        method.SetAsDefault();

        // Act
        method.UnsetDefault();

        // Assert
        Assert.That(method.IsDefault, Is.False);
    }
}
