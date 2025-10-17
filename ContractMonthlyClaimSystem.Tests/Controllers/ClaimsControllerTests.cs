using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Moq;
using ContractMonthlyClaimSystem.Controllers;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Tests.Controllers
{
    public class ClaimsControllerTests
    {
        private readonly Mock<IWebHostEnvironment> _hostingEnvironmentMock;
        private readonly ClaimsController _controller;
        private readonly Mock<ISession> _sessionMock;

        public ClaimsControllerTests()
        {
            _hostingEnvironmentMock = new Mock<IWebHostEnvironment>();
            _sessionMock = new Mock<ISession>();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = _sessionMock.Object;

            _controller = new ClaimsController(_hostingEnvironmentMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                }
            };
        }

        [Fact]
        public void Index_UserNotLoggedIn_RedirectsToLogin()
        {
            // Arrange - No user in session

            // Act
            var result = _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
            Assert.Equal("Account", redirectResult.ControllerName);
        }

        [Fact]
        public void Index_UserLoggedIn_ReturnsViewWithClaims()
        {
            // Arrange
            var userEmail = "lecturer@university.edu";
            SetupUserSession(userEmail);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<MonthlyClaim>>(viewResult.Model);
            Assert.NotNull(model);
            Assert.All(model, claim => Assert.Equal(userEmail, claim.UserId));
        }

        [Fact]
        public void Create_Get_UserNotLoggedIn_RedirectsToLogin()
        {
            // Act
            var result = _controller.Create();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
        }

        [Fact]
        public void Create_Get_UserLoggedIn_ReturnsView()
        {
            // Arrange
            SetupUserSession("lecturer@university.edu");

            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_Post_ValidClaim_RedirectsToIndex()
        {
            // Arrange
            SetupUserSession("lecturer@university.edu");
            var claim = new MonthlyClaim
            {
                Month = 3,
                Year = 2024,
                ClaimItems = new List<ClaimItem>
                {
                    new ClaimItem { Date = DateTime.Now, HoursWorked = 8, Module = "CS101", Description = "Lecture", Amount = 400 }
                }
            };

            // Act
            var result = _controller.Create(claim, "submit", null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public void Create_Post_InvalidClaim_ReturnsView()
        {
            // Arrange
            SetupUserSession("lecturer@university.edu");
            var claim = new MonthlyClaim(); // Invalid claim
            _controller.ModelState.AddModelError("Month", "Month is required");

            // Act
            var result = _controller.Create(claim, "submit", null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(claim, viewResult.Model);
        }

        private void SetupUserSession(string email)
        {
            _sessionMock.Setup(s => s.GetString("UserEmail")).Returns(email);
        }
    }
}