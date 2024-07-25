using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Domain.Enums;
using PAMAi.Infrastructure.Identity.Services;

namespace PAMAi.Tests.Unit.Infrastructure.Identity.Services;

internal class CurrentUserTests
{
    [Test]
    public void Should_Succeed()
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, ApplicationRole.SuperAdmin.ToString()),
        ];
        var mock = new Mock<IHttpContextAccessor>();
        mock.Setup(a => a.HttpContext.User.Claims)
            .Returns(claims);
        IHttpContextAccessor httpContextAccessor = mock.Object;
        ICurrentUser currentUser = new CurrentUser(httpContextAccessor);

        Assert.Multiple(() =>
        {
            Assert.That(currentUser.Any, Is.True, "Current user wrongly indicates that there's no logged in user");
            Assert.That(currentUser.UserId, Is.Not.Null, "Current user's ID is null");
            Assert.That(currentUser.Role, Is.EqualTo(ApplicationRole.SuperAdmin), "Current user's role is incorrect");
        });
    }

    [Test]
    public void Should_Fail()
    {
        IHttpContextAccessor httpContextAccessor = Mock.Of<IHttpContextAccessor>();
        ICurrentUser currentUser = new CurrentUser(httpContextAccessor);

        Assert.Multiple(() =>
        {
            Assert.That(currentUser.Any, Is.False, "Current user wrongly indicates that there's a logged in user");
            Assert.That(currentUser.UserId, Is.Null, "Current user's ID is not null");
            Assert.That(currentUser.Role, Is.Null, "Current user's role is not null");
        });
    }
}
