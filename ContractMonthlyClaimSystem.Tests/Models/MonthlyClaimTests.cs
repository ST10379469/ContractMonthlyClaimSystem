using ContractMonthlyClaimSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractMonthlyClaimSystem.Tests.Models
{
    public class MonthlyClaimTests
    {
        [Fact]
        public void MonthlyClaim_TotalAmount_ShouldCalculateCorrectly()
        {
            // Arrange
            var claim = new MonthlyClaim
            {
                ClaimItems = new List<ClaimItem>
                {
                    new ClaimItem { Amount = 100 },
                    new ClaimItem { Amount = 200 },
                    new ClaimItem { Amount = 300 }
                }
            };

            // Act
            var totalAmount = claim.TotalAmount;

            // Assert
            Assert.Equal(600, totalAmount);
        }

        [Fact]
        public void MonthlyClaim_NoClaimItems_ShouldReturnZeroTotal()
        {
            // Arrange
            var claim = new MonthlyClaim
            {
                ClaimItems = new List<ClaimItem>()
            };

            // Act
            var totalAmount = claim.TotalAmount;

            // Assert
            Assert.Equal(0, totalAmount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        [InlineData(-1)]
        public void MonthlyClaim_InvalidMonth_ShouldFailValidation(int invalidMonth)
        {
            // Arrange
            var claim = new MonthlyClaim
            {
                Month = invalidMonth,
                Year = 2024,
                UserId = "test@university.edu"
            };

            var context = new ValidationContext(claim);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("Month"));
        }
    }
}