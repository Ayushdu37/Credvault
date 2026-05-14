using Moq;
using NUnit.Framework;
using IdentityService.Application.Queries.GetUserProfile;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;

namespace CredVault.Tests.IdentityService;

[TestFixture]
public class IdentityQueryHandlerTests
{
    private Mock<IUserRepository> _userRepo = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepo = new Mock<IUserRepository>();
    }

    [Test]
    public async Task GetUserProfile_ExistingUser_ReturnsProfile()
    {
        // Arrange
        var user = User.Create("user@test.com", "hash", "Test User", "9876543210");
        user.VerifyEmail();

        _userRepo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new GetUserProfileQueryHandler(_userRepo.Object);
        var query = new GetUserProfileQuery(user.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Email, Is.EqualTo("user@test.com"));
        Assert.That(result.Data.FullName, Is.EqualTo("Test User"));
        Assert.That(result.Data.IsEmailVerified, Is.True);
    }

    [Test]
    public async Task GetUserProfile_UserNotFound_ReturnsFailure()
    {
        // Arrange
        _userRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new GetUserProfileQueryHandler(_userRepo.Object);

        // Act
        var result = await handler.Handle(new GetUserProfileQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("not found"));
    }
}
