using Moq;
using NUnit.Framework;
using IdentityService.Application.Commands.LoginUser;
using IdentityService.Application.Abstraction;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using CredVault.Shared.Contracts.Enums;

namespace CredVault.Tests.IdentityService;

[TestFixture]
public class LoginUserCommandHandlerTests
{
    private Mock<IUserRepository> _userRepo = null!;
    private Mock<IRefreshTokenRepository> _refreshTokenRepo = null!;
    private Mock<IPasswordHasher> _passwordHasher = null!;
    private Mock<ITokenService> _tokenService = null!;
    private LoginUserCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepo = new Mock<IUserRepository>();
        _refreshTokenRepo = new Mock<IRefreshTokenRepository>();
        _passwordHasher = new Mock<IPasswordHasher>();
        _tokenService = new Mock<ITokenService>();
        _handler = new LoginUserCommandHandler(
            _userRepo.Object, _refreshTokenRepo.Object,
            _passwordHasher.Object, _tokenService.Object);
    }

    [Test]
    public async Task Login_ValidCredentials_ReturnsAccessToken()
    {
        // Arrange
        var user = User.Create("user@test.com", "hashed-pw", "Test User", "123");
        user.VerifyEmail();

        _userRepo.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(user.PasswordHash, "password123")).Returns(true);
        _tokenService.Setup(t => t.GenerateAccessToken(user)).Returns("jwt-token");
        _tokenService.Setup(t => t.GenerateRefreshToken()).Returns("refresh-token");
        _refreshTokenRepo.Setup(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new LoginUserCommand("user@test.com", "password123", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.AccessToken, Is.EqualTo("jwt-token"));
        Assert.That(result.Data.RefreshToken, Is.EqualTo("refresh-token"));
    }

    [Test]
    public async Task Login_NonExistentEmail_ReturnsFailure()
    {
        // Arrange
        _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(
            new LoginUserCommand("noone@test.com", "pw", null), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task Login_WrongPassword_ReturnsFailure()
    {
        // Arrange
        var user = User.Create("user@test.com", "hashed-pw", "Test", "123");
        user.VerifyEmail();

        _userRepo.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(It.IsAny<string>(), "wrong-pw")).Returns(false);

        // Act
        var result = await _handler.Handle(
            new LoginUserCommand("user@test.com", "wrong-pw", null), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task Login_UnverifiedAccount_ReturnsFailure()
    {
        // Arrange — user not verified (default status = PendingVerification)
        var user = User.Create("user@test.com", "hashed-pw", "Test", "123");

        _userRepo.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(user.PasswordHash, "password123")).Returns(true);

        // Act
        var result = await _handler.Handle(
            new LoginUserCommand("user@test.com", "password123", null), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task Login_SuspendedAccount_ReturnsFailure()
    {
        // Arrange
        var user = User.Create("user@test.com", "hashed-pw", "Test", "123");
        user.VerifyEmail();
        user.Suspend();

        _userRepo.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(user.PasswordHash, "password123")).Returns(true);

        // Act
        var result = await _handler.Handle(
            new LoginUserCommand("user@test.com", "password123", null), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
    }

    // ─── INTENTIONALLY FAILING TESTS ─────────────────────────────────

    [Test]
    public async Task Login_WrongPassword_ShouldFail_ButExpectsSuccess()
    {
        // Arrange — user exists, password is WRONG
        var user = User.Create("user@test.com", "hashed-pw", "Test User", "123");
        user.VerifyEmail();

        _userRepo.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(It.IsAny<string>(), "wrong-pw")).Returns(false);

        var command = new LoginUserCommand("user@test.com", "wrong-pw", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert — WRONG: expecting success when password is invalid → WILL FAIL
        Assert.That(result.Success, Is.True, "BUG: Login should NOT succeed with incorrect credentials");
    }

    [Test]
    public async Task Login_SuspendedAccount_ShouldFail_ButExpectsToken()
    {
        // Arrange — account is suspended
        var user = User.Create("suspended@test.com", "hashed-pw", "Suspended User", "123");
        user.VerifyEmail();
        user.Suspend();

        _userRepo.Setup(r => r.GetByEmailAsync("suspended@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(user.PasswordHash, "password123")).Returns(true);

        var command = new LoginUserCommand("suspended@test.com", "password123", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert — WRONG: expecting a token from a suspended account → WILL FAIL
        Assert.That(result.Data, Is.Not.Null, "BUG: Suspended account should NOT return auth tokens");
        Assert.That(result.Data!.AccessToken, Is.Not.Empty);
    }
}
