using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;

namespace BupaTest.IntegrationTest
{
    [TestFixture]
    public class BookControllerTest
    {
        private readonly HttpClient _client;
        private WebApplicationFactory<Program> _factory;

        public BookControllerTest()
        {
             _factory = new WebApplicationFactory<Program>(); // Your actual startup class
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task GetBooks_WithHardcoverFilter_ReturnsFilteredBooks()
        {
            // Arrange: Set the filter query parameter
            var url = "/api/books?filter=hardcover";

            // Act: Send a GET request to the API
            var response = await _client.GetAsync(url);

            // Assert: Verify the API response
            response.EnsureSuccessStatusCode(); // Status 200 OK
            var content = await response.Content.ReadAsStringAsync();
            var books = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(content);

            Assert.AreEqual(2, books.Count());  // Expect two hardcover books
        }

        [Test]
        public async Task GetBooks_WithInvalidFilter_ReturnsNotFound()
        {
            // Arrange
            var url = "/api/books?filter=nonexistent";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.AreEqual(404, (int)response.StatusCode); // Expect 404 Not Found
        }

        [Test]
        public async Task GetBooks_WhenErrorOccurs_ReturnsInternalServerError()
        {
            // Arrange: Simulate server-side error by calling a non-existent API or invalid data
            var url = "/api/books?filter=error"; // You can simulate an error condition

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.AreEqual(500, (int)response.StatusCode); // Expect 500 Internal Server Error
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the client after each test
            _client.Dispose();
        }
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client.Dispose();
            _factory.Dispose();
            // Any one-time cleanup can go here
        }
    }


}
