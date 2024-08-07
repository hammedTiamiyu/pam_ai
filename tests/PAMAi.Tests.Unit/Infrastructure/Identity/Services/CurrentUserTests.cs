using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Storage;
using PAMAi.Domain.Entities;
using PAMAi.Domain.Enums;
using PAMAi.Infrastructure.Identity.Services;

namespace PAMAi.Tests.Unit.Infrastructure.Identity.Services;

internal class CurrentUserTests
{
    [Test]
    public void Should_Succeed()
    {
        UserProfile userProfile = new()
        {
            Id = Guid.NewGuid(),
        };
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, ApplicationRole.SuperAdmin.ToString()),
        ];

        var mock = new Mock<IHttpContextAccessor>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        mock
            .Setup(a => a.HttpContext.User.Claims)
            .Returns(claims);
        unitOfWorkMock
            .Setup(u => u.UserProfiles.FindAsync(It.IsAny<string>(), CancellationToken.None))
            .Returns(Task.FromResult<UserProfile?>(userProfile));

        IHttpContextAccessor httpContextAccessor = mock.Object;
        IUnitOfWork unitOfWork = unitOfWorkMock.Object;
        ICurrentUser currentUser = new CurrentUser(httpContextAccessor, unitOfWork);

        unitOfWorkMock.Verify(mock => mock.UserProfiles.FindAsync(It.IsAny<string>(), CancellationToken.None), Times.AtLeastOnce);
        Assert.Multiple(() =>
        {
            Assert.That(currentUser.Any, Is.True, "Current user wrongly indicates that there's no logged in user");
            Assert.That(currentUser.UserId, Is.Not.Null, "Current user's ID is null");
            Assert.That(currentUser.UserProfileId, Is.Not.Null, "Current user's role is incorrect");
            Assert.That(currentUser.Role, Is.EqualTo(ApplicationRole.SuperAdmin), "Current user's role is incorrect");
        });
    }

    [Test]
    public void Should_Fail()
    {
        IHttpContextAccessor httpContextAccessor = Mock.Of<IHttpContextAccessor>();
        IUnitOfWork unitOfWork = Mock.Of<IUnitOfWork>();
        ICurrentUser currentUser = new CurrentUser(httpContextAccessor, unitOfWork);

        Assert.Multiple(() =>
        {
            Assert.That(currentUser.Any, Is.False, "Current user wrongly indicates that there's a logged in user");
            Assert.That(currentUser.UserId, Is.Null, "Current user's ID is not null");
            Assert.That(currentUser.UserProfileId, Is.Null, "Current user's ID is not null");
            Assert.That(currentUser.Role, Is.Null, "Current user's role is not null");
        });
    }
}
