using System.Text.Json;
using System;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;
using LogProxyAPI;
using LogProxyAPI.Model;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Net.Http;

namespace LogProxyTest
{
    [Collection("Integration Tests")]
    public class ProxyControllerTests
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ProxyControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ReturnsSuccessAndGetMessages()
        {
            var client = _factory.CreateClient();

            string userName = "sinan";
            string password = "semiz";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                AuthenticationSchemes.Basic.ToString(),
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"))
                );

            var response = await client.GetAsync("/proxy");

            response.EnsureSuccessStatusCode();
            Assert.NotNull(response.Content);
            var responseObject = JsonSerializer.Deserialize<IEnumerable<LogIncoming>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { });
            Assert.NotEmpty(responseObject);
        }

        [Fact]
        public async Task Post_CouldSend()
        {
            var client = _factory.CreateClient();

            string userName = "sinan";
            string password = "semiz";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                AuthenticationSchemes.Basic.ToString(),
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"))
                );

            var response = await client.PostAsync("/proxy", 
                new StringContent(JsonSerializer.Serialize(
                    new LogIncoming { title = "Test Call", text = "This is a test call"}
                    ), Encoding.UTF8, "application/json")
                
                );

            response.EnsureSuccessStatusCode();
        }

    }
}
