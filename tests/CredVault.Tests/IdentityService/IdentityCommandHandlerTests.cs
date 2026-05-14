using Moq;
using NUnit.Framework;
using IdentityService.Application.Commands.RefreshToken;
using IdentityService.Application.Commands.VerifyEmail;
using IdentityService.Application.Commands.SendOTP;
using IdentityService.Application.Commands.VerifyOTP;
using IdentityService.Application.Commands.ResetPassword;
using IdentityService.Application.Abstraction;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using CredVault.Shared.Contracts.Enums;
using DomainRefreshToken = global::IdentityService.Domain.Entities.RefreshToken;

namespace CredVault.Tests.IdentityService;

[TestFixture]
public class IdentityCommandHandlerTests
{
    // ─── RefreshTokenCommandHandler ──────────────────────────────────

    [Test]
    public async Task RefreshToken_ValidToken_ReturnsNewTokenPair()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = User.Create("user@test.com", "hash", "User", "123");
        typeof(User).GetProperty("Id")!.SetValue(user, userId);

        var existingToken = DomainRefreshToken.Create(
            userId, "old-refresh-token", deviceInfo: "Chrome");

        var refreshTokenRepo = new Mock<IRefreshTokenRepository>();
        var userRepo = new Mock<IUserRepository>();
        var tokenService = new Mock<ITokenService>();

        refreshTokenRepo.Setup(r => r.GetByTokenAsync("old-refresh-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingToken);
        userRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        refreshTokenRepo.Setup(r => r.RevokeAsync(existingToken, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        tokenService.Setup(t => t.GenerateAccessToken(user)).Returns("new-jwt");
        tokenService.Setup(t => t.GenerateRefreshToken()).Returns("new-refresh");
        refreshTokenRepo.Setup(r => r.AddAsync(
            It.IsAny<DomainRefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new RefreshTokenCommandHandler(refreshTokenRepo.Object, userRepo.Object, tokenService.Object);

        // Act
        var result = await handler.Handle(new RefreshTokenCommand("old-refresh-token"), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data!.AccessToken, Is.EqualTo("new-jwt"));
        Assert.That(result.Data.RefreshToken, Is.EqualTo("new-refresh"));
        refreshTokenRepo.Verify(r => r.RevokeAsync(existingToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task RefreshToken_InvalidToken_ReturnsFailure()
    {
        // Arrange
        var refreshTokenRepo = new Mock<IRefreshTokenRepository>();
        var userRepo = new Mock<IUserRepository>();
        var tokenService = new Mock<ITokenService>();

        refreshTokenRepo.Setup(r => r.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainRefreshToken?)null);

        var handler = new RefreshTokenCommandHandler(refreshTokenRepo.Object, userRepo.Object, tokenService.Object);

        // Act
        var result = await handler.Handle(new RefreshTokenCommand("invalid-token"), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid or expired"));
    }

    // ─── VerifyEmailCommandHandler ───────────────────────────────────

    [Test]
    public async Task VerifyEmail_ValidOTP_VerifiesUser()
    {
        // Arrange
        var user = User.Create("user@test.com", "hash", "User", "123");
        var otp = OTPCode.Create(user.Id, "123456", OTPPurpose.EmailVerification, expiryMinutes: 5);

        var userRepo = new Mock<IUserRepository>();
        var otpRepo = new Mock<IOTPCodeRepository>();

        userRepo.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        otpRepo.Setup(r => r.GetLatestAsync(user.Id, OTPPurpose.EmailVerification, It.IsAny<CancellationToken>()))
            .ReturnsAsync(otp);
        otpRepo.Setup(r => r.MarkUsedAsync(otp, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        userRepo.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new VerifyEmailCommandHandler(userRepo.Object, otpRepo.Object);

        // Act
        var result = await handler.Handle(new VerifyEmailCommand("user@test.com", "123456"), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(user.IsEmailVerified, Is.True);
    }

    [Test]
    public async Task VerifyEmail_AlreadyVerified_ReturnsFailure()
    {
        // Arrange
        var user = User.Create("user@test.com", "hash", "User", "123");
        user.VerifyEmail();

        var userRepo = new Mock<IUserRepository>();
        var otpRepo = new Mock<IOTPCodeRepository>();

        userRepo.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var handler = new VerifyEmailCommandHandler(userRepo.Object, otpRepo.Object);

        // Act
        var result = await handler.Handle(new VerifyEmailCommand("user@test.com", "123456"), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("already verified"));
    }

    [Test]
    public async Task VerifyEmail_WrongOTPCode_ReturnsFailure()
    {
        // Arrange
        var user = User.Create("user@test.com", "hash", "User", "123");
        var otp = OTPCode.Create(user.Id, "123456", OTPPurpose.EmailVerification, expiryMinutes: 5);

        var userRepo = new Mock<IUserRepository>();
        var otpRepo = new Mock<IOTPCodeRepository>();

        userRepo.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        otpRepo.Setup(r => r.GetLatestAsync(user.Id, OTPPurpose.EmailVerification, It.IsAny<CancellationToken>()))
            .ReturnsAsync(otp);

        var handler = new VerifyEmailCommandHandler(userRepo.Object, otpRepo.Object);

        // Act
        var result = await handler.Handle(new VerifyEmailCommand("user@test.com", "999999"), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid or expired"));
    }

    // ─── SendOTPCommandHandler ───────────────────────────────────────

    [Test]
    public async Task SendOTP_ValidUser_CreatesOTPAndPublishesEvent()
    {
        // Arrange
        var user = User.Create("user@test.com", "hash", "User", "123");

        var userRepo = new Mock<IUserRepository>();
        var otpRepo = new Mock<IOTPCodeRepository>();
        var eventPublisher = new Mock<IEventPublisher>();

        userRepo.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        otpRepo.Setup(r => r.AddAsync(It.IsAny<OTPCode>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        eventPublisher.Setup(e => e.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new SendOTPCommandHandler(userRepo.Object, otpRepo.Object, eventPublisher.Object);

        // Act
        var result = await handler.Handle(
            new SendOTPCommand("user@test.com", OTPPurpose.EmailVerification), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        otpRepo.Verify(r => r.AddAsync(It.IsAny<OTPCode>(), It.IsAny<CancellationToken>()), Times.Once);
        eventPublisher.Verify(e => e.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SendOTP_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var userRepo = new Mock<IUserRepository>();
        var otpRepo = new Mock<IOTPCodeRepository>();
        var eventPublisher = new Mock<IEventPublisher>();

        userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new SendOTPCommandHandler(userRepo.Object, otpRepo.Object, eventPublisher.Object);

        // Act
        var result = await handler.Handle(
            new SendOTPCommand("noone@test.com", OTPPurpose.EmailVerification), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("not found"));
    }

    // ─── ResetPasswordCommandHandler ─────────────────────────────────

    [Test]
    public async Task ResetPassword_ValidOTP_ChangesPassword()
    {
        // Arrange
        var user = User.Create("user@test.com", "old-hash", "User", "123");
        var otp = OTPCode.Create(user.Id, "654321", OTPPurpose.PasswordReset, expiryMinutes: 5);

        var userRepo = new Mock<IUserRepository>();
        var otpRepo = new Mock<IOTPCodeRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();

        userRepo.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        otpRepo.Setup(r => r.GetLatestAsync(user.Id, OTPPurpose.PasswordReset, It.IsAny<CancellationToken>()))
            .ReturnsAsync(otp);
        otpRepo.Setup(r => r.MarkUsedAsync(otp, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        passwordHasher.Setup(h => h.Hash("NewPassword123")).Returns("new-hash");
        userRepo.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new ResetPasswordCommandHandler(userRepo.Object, otpRepo.Object, passwordHasher.Object);

        // Act
        var result = await handler.Handle(
            new ResetPasswordCommand("user@test.com", "654321", "NewPassword123"), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(user.PasswordHash, Is.EqualTo("new-hash"));
    }

    [Test]
    public async Task ResetPassword_InvalidOTP_ReturnsFailure()
    {
        // Arrange
        var user = User.Create("user@test.com", "old-hash", "User", "123");
        var otp = OTPCode.Create(user.Id, "654321", OTPPurpose.PasswordReset, expiryMinutes: 5);

        var userRepo = new Mock<IUserRepository>();
        var otpRepo = new Mock<IOTPCodeRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();

        userRepo.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        otpRepo.Setup(r => r.GetLatestAsync(user.Id, OTPPurpose.PasswordReset, It.IsAny<CancellationToken>()))
            .ReturnsAsync(otp);

        var handler = new ResetPasswordCommandHandler(userRepo.Object, otpRepo.Object, passwordHasher.Object);

        // Act
        var result = await handler.Handle(
            new ResetPasswordCommand("user@test.com", "000000", "NewPassword123"), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("Invalid or expired"));
    }
}
