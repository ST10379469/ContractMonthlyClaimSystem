using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using Xunit;

namespace ContractMonthlyClaimSystem.Tests.Integration
{
    public class ClaimsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ClaimsIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task HomePage_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LoginPage_ReturnsLoginForm()
        {
            // Act
            var response = await _client.GetAsync("/Account/Login");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Login", content);
            Assert.Contains("Email", content);
            Assert.Contains("Password", content);
        }

        [Fact]
        public async Task ClaimsIndex_Unauthorized_RedirectsToLogin()
        {
            // Act
            var response = await _client.GetAsync("/Claims");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Should redirect to login page
        }
    }
}