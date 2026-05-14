using Moq;
using NUnit.Framework;
using IdentityService.Application.Commands.RegisterUser;
using IdentityService.Application.Abstraction;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using CredVault.Shared.Contracts.Enums;

namespace CredVault.Tests.IdentityService;

[TestFixture]
public class RegisterUserCommandHandlerTests
{
    private Mock<IUserRepository> _userRepo = null!;
    private Mock<IPasswordHasher> _passwordHasher = null!;
    private Mock<IEventPublisher> _eventPublisher = null!;
    private RegisterUserCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepo = new Mock<IUserRepository>();
        _passwordHasher = new Mock<IPasswordHasher>();
        _eventPublisher = new Mock<IEventPublisher>();
        _handler = new RegisterUserCommandHandler(
            _userRepo.Object, _passwordHasher.Object, _eventPublisher.Object);
    }

    [Test]
    public async Task Register_NewUser_ReturnsSuccessAndPublishesEvent()
    {
        // Arrange
        _userRepo.Setup(r => r.GetByEmailAsync("new@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwordHasher.Setup(h => h.Hash("SecurePassword1")).Returns("hashed-pw");
        _userRepo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _eventPublisher.Setup(e => e.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new RegisterUserCommand(
            "new@test.com", "SecurePassword1", "New User", "9876543210");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        _userRepo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisher.Verify(e => e.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Register_ExistingVerifiedEmail_ReturnsFailure()
    {
        // Arrange
        var existingUser = User.Create("existing@test.com", "hash", "Existing", "123");
        existingUser.VerifyEmail();

        _userRepo.Setup(r => r.GetByEmailAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        var command = new RegisterUserCommand(
            "existing@test.com", "pw", "Name", "123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task Register_ExistingUnverifiedEmail_ReturnsSuccessWithRedirectMessage()
    {
        // Arrange — existing user but not verified triggers OTP resend flow
        var unverified = User.Create("unverified@test.com", "hash", "Unverified", "123");

        _userRepo.Setup(r => r.GetByEmailAsync("unverified@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(unverified);
        _eventPublisher.Setup(e => e.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new RegisterUserCommand(
            "unverified@test.com", "pw", "Name", "123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
    }
}
