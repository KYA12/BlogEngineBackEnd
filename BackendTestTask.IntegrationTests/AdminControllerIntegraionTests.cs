using BackendTestTask.DTO;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace BackendTestTask.IntegrationTests
{
    public class AdminControllerIntegrationTests : IClassFixture<TestingWebAppFactory<Startup>>
    {
        private readonly HttpClient Client;
        public AdminControllerIntegrationTests(TestingWebAppFactory<Startup> factory)
        {
            Client = factory.CreateClient();
            Client.DefaultRequestHeaders.TryAddWithoutValidation("ApiKey", "209e75ab-674b-41a3-92ea-eae383aba37d");
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Fact]
        public async Task GetPostListAsync_WhenGetOnlyAdmin_ThenReturns200Ok()
        {
            // Arrange
            var request = "/api/admin";
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, request);
          
            // Act
            var response = await Client.SendAsync(req);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetPostByIdAsync_WhenGetOnlyAdmin_ThenReturns200Ok()
        {
            // Arrange
            var request = "/api/admin?id=1";
           
            // Act
            var response = await Client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task AddPostAsync_WhenGetOnlyAdmin_ThenReturns200Ok()
        {
            // Arrange
            string request = "/api/admin";
            PostDTO post = new PostDTO
            {
                Title = "Microsoft",
                Description = "Hello",
                Body = "Hey",
                Tags = "Tags",
                Created = DateTime.Now,
                Comments = null
            };
            
            // Act
            var response = await Client.PostAsync(request, JsonSerializeHelper.GetStringContent(post));

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PutPostAsync_WhenGetOnlyAdmin_ThenReturns200Ok()
        {
            // Arrange
            string request = "/api/admin/1";

            PostDTO post = new PostDTO
            {
                Title = "Microsoft",
                Description = "Hello",
                Body = "Body",
                Tags = "Tags",
                Created = DateTime.Now,
                Comments = null
            };

            // Act
            var response = await Client.PutAsync(request, JsonSerializeHelper.GetStringContent(post));

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeletePostAsync_WhenGetOnlyAdmin_ThenReturns200Ok()
        {

            // Arrange
            var request = $"/api/admin/1";
         
            // Act
            var response = await Client.DeleteAsync(new Uri($"api/admin/{1}", UriKind.Relative));

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
