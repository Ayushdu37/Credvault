using NUnit.Framework;
using IdentityService.Domain.Entities;
using CredVault.Shared.Contracts.Enums;

namespace CredVault.Tests.IdentityService;

[TestFixture]
public class RefreshTokenEntityTests
{
    [Test]
    public void Create_SetsDefaultExpiry7Days()
    {
        // Arrange & Act
        var token = RefreshToken.Create(Guid.NewGuid(), "token-value");

        // Assert
        Assert.That(token.ExpiresAt, Is.GreaterThan(DateTime.UtcNow.AddDays(6)));
        Assert.That(token.IsActive, Is.True);
        Assert.That(token.IsRevoked, Is.False);
        Assert.That(token.IsExpired, Is.False);
    }

    [Test]
    public void Create_CustomExpiry_SetsCorrectExpiration()
    {
        // Arrange & Act
        var token = RefreshToken.Create(Guid.NewGuid(), "token-value", expiryDays: 1);

        // Assert
        Assert.That(token.ExpiresAt, Is.LessThan(DateTime.UtcNow.AddDays(2)));
    }

    [Test]
    public void Revoke_SetsRevokedAtAndDeactivatesToken()
    {
        // Arrange
        var token = RefreshToken.Create(Guid.NewGuid(), "token-value");

        // Act
        token.Revoke();

        // Assert
        Assert.That(token.IsRevoked, Is.True);
        Assert.That(token.IsActive, Is.False);
        Assert.That(token.RevokedAt, Is.Not.Null);
    }

    [Test]
    public void Create_StoresDeviceInfo()
    {
        // Arrange & Act
        var token = RefreshToken.Create(Guid.NewGuid(), "token-value", deviceInfo: "Chrome/Windows");

        // Assert
        Assert.That(token.DeviceInfo, Is.EqualTo("Chrome/Windows"));
    }
}

[TestFixture]
public class OTPCodeEntityTests
{
    [Test]
    public void Create_SetsCorrectDefaults()
    {
        // Arrange & Act
        var otp = OTPCode.Create(Guid.NewGuid(), "123456", OTPPurpose.EmailVerification);

        // Assert
        Assert.That(otp.Code, Is.EqualTo("123456"));
        Assert.That(otp.Purpose, Is.EqualTo(OTPPurpose.EmailVerification));
        Assert.That(otp.IsUsed, Is.False);
        Assert.That(otp.IsValid, Is.True);
        Assert.That(otp.ExpiresAt, Is.GreaterThan(DateTime.UtcNow));
    }

    [Test]
    public void MarkUsed_SetsIsUsedAndInvalidatesOTP()
    {
        // Arrange
        var otp = OTPCode.Create(Guid.NewGuid(), "123456", OTPPurpose.EmailVerification);

        // Act
        otp.MarkUsed();

        // Assert
        Assert.That(otp.IsUsed, Is.True);
        Assert.That(otp.IsValid, Is.False);
    }

    [Test]
    public void IsExpired_PastExpiry_ReturnsTrue()
    {
        // Arrange — create OTP with 0 minute expiry (immediately expired)
        var otp = OTPCode.Create(Guid.NewGuid(), "123456", OTPPurpose.PasswordReset, expiryMinutes: 0);

        // Assert — with 0 minutes, ExpiresAt = UtcNow, so it should be expired
        Assert.That(otp.IsExpired, Is.True);
        Assert.That(otp.IsValid, Is.False);
    }
}
