using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using WajeSmartAssessment.Application.Helpers;

namespace WajeSmartAssessment.Tests.Helpers
{
    public class TestUserHelper
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public TestUserHelper()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            UserHelper.Configure(_mockHttpContextAccessor.Object);
        }

        public void SetupCurrentUser(string? id = null, string? role = null, string? username = null)
        {
            var claims = new List<Claim>();
            if (id != null)
                claims.Add(new Claim(ClaimTypes.NameIdentifier, id));
            if (username != null)
                claims.Add(new Claim(ClaimTypes.Name, username));
            if (role != null)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);

            _mockHttpContextAccessor.Setup(m => m.HttpContext).Returns(mockHttpContext.Object);
        }

        public void SetupAdminUser()
        {
            SetupCurrentUser(Guid.NewGuid().ToString(), "admin", "Admin");
        }

        public UserPrincipal SetupRegularAuthor(string id)
        {
            SetupCurrentUser(id, "Author", "user");
            return new UserPrincipal
            {
                Id = id,
                Username = "username",
                Role = "Author"
            };
        }

        public void SetupUnauthenticatedUser()
        {
            SetupCurrentUser();
        }
    }
}