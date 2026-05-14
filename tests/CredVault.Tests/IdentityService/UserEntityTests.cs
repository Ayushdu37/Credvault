using NUnit.Framework;
using IdentityService.Domain.Entities;
using CredVault.Shared.Contracts.Enums;

namespace CredVault.Tests.IdentityService;

[TestFixture]
public class UserEntityTests
{
    [Test]
    public void Create_SetsDefaultStatusToPendingVerification()
    {
        // Arrange & Act
        var user = User.Create("test@example.com", "hash", "Test User", "1234567890");

        // Assert
        Assert.That(user.Status, Is.EqualTo(UserStatus.PendingVerification));
        Assert.That(user.IsEmailVerified, Is.False);
        Assert.That(user.Role, Is.EqualTo(UserRole.User));
        Assert.That(user.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Create_NormalizesEmailToLowercase()
    {
        // Arrange & Act
        var user = User.Create("Test@EXAMPLE.com", "hash", "Test", "123");

        // Assert
        Assert.That(user.Email, Is.EqualTo("test@example.com"));
    }

    [Test]
    public void VerifyEmail_SetsActiveStatusAndVerifiedFlag()
    {
        // Arrange
        var user = User.Create("test@example.com", "hash", "Test", "123");

        // Act
        user.VerifyEmail();

        // Assert
        Assert.That(user.IsEmailVerified, Is.True);
        Assert.That(user.Status, Is.EqualTo(UserStatus.Active));
        Assert.That(user.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public void UpdatePassword_ChangesPasswordHash()
    {
        // Arrange
        var user = User.Create("test@example.com", "old-hash", "Test", "123");

        // Act
        user.UpdatePassword("new-hash");

        // Assert
        Assert.That(user.PasswordHash, Is.EqualTo("new-hash"));
        Assert.That(user.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public void Suspend_SetsStatusToSuspended()
    {
        // Arrange
        var user = User.Create("test@example.com", "hash", "Test", "123");
        user.VerifyEmail();

        // Act
        user.Suspend();

        // Assert
        Assert.That(user.Status, Is.EqualTo(UserStatus.Suspended));
    }

    [Test]
    public void Activate_SetsStatusToActive()
    {
        // Arrange
        var user = User.Create("test@example.com", "hash", "Test", "123");
        user.Suspend();

        // Act
        user.Activate();

        // Assert
        Assert.That(user.Status, Is.EqualTo(UserStatus.Active));
    }
}
