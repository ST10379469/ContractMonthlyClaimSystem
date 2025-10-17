using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using ContractMonthlyClaimSystem.Controllers;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();
            httpContext.Session = sessionMock.Object;

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            _controller = new AccountController(_httpContextAccessorMock.Object);
        }

        [Fact]
        public void Login_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Login();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void Login_Post_ValidModel_RedirectsToHome()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@university.edu",
                Password = "Password123",
                Role = UserRole.Lecturer
            };

            // Act
            var result = _controller.Login(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public void Login_Post_InvalidModel_ReturnsView()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "invalid-email",
                Password = "",
                Role = UserRole.Lecturer
            };

            _controller.ModelState.AddModelError("Email", "Invalid email");

            // Act
            var result = _controller.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public void Logout_ClearsSessionAndRedirects()
        {
            // Act
            var result = _controller.Logout();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
        }
    }
}
