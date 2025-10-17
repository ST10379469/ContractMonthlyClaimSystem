using ContractMonthlyClaimSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Tests.Models
{
    public class LoginViewModelTests
    {
        [Fact]
        public void LoginViewModel_ValidData_ShouldPassValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@university.edu",
                Password = "Password123",
                Role = UserRole.Lecturer
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Theory]
        [InlineData("")]
        [InlineData("invalid-email")]
        [InlineData(null)]
        public void LoginViewModel_InvalidEmail_ShouldFailValidation(string email)
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = email,
                Password = "Password123",
                Role = UserRole.Lecturer
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void LoginViewModel_MissingPassword_ShouldFailValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@university.edu",
                Password = "",
                Role = UserRole.Lecturer
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }
    }
}