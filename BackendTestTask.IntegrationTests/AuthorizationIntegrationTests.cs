using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ShouldBeAssertions;

namespace BackendTestTask.IntegrationTests
{  
    public class AuthorizationIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        public AuthorizationIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        [Theory]
        [InlineData("admin")]
        public async Task GivenUnauthorizedCall_WhenGetOnlyAdmin_ThenReturns401Unathorized(string action)
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/{action}");
            var apiKey = "ApiKey"; 
            request.Headers.Add("ApiKey", apiKey);
            
            // Act 
            var response = await httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.Content.Headers.ContentType.ToString().ShouldHaveValuesOf("application/problem+json");
            responseContent.ShouldLookLike("{\"type\":\"https://httpstatuses.com/401\",\"title\":\"Unauthorized\",\"status\":401}"); 
            response.StatusCode.ShouldHaveValuesOf(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GivenAuthorizedCall_WhenGetOnlyAdmin_ThenReturns200Ok()
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "api/admin/");
            var apiKey = "209e75ab-674b-41a3-92ea-eae383aba37d";
            request.Headers.Add("ApiKey", apiKey);
           
            // Act
            var response = await httpClient.SendAsync(request);
            
            // Assert
            response.StatusCode.ShouldHaveValuesOf(HttpStatusCode.OK);
        }

    }
}
