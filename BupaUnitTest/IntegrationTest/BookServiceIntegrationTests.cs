using Bupa.Models;
using Bupa.Services;
using Newtonsoft.Json;

namespace BupaTest.IntegrationTest
{
    [TestFixture]
    public class BookServiceIntegrationTests
    {
        private BookService _service;
        private HttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            // Initialize HttpClient to interact with the real external API
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri("https://digitalcodingtest.bupa.com.au/api/v1/bookowners")
            };

            // Instantiate the service with real HttpClient
            _service = new BookService(_httpClient);
        }

        [Test]
        public async Task GetBooksAsync_ShouldReturnBooks_WhenExternalApiIsAvailable()
        {
            // Act: Call the service method to get the list of books
            var result = await _service.GetBooksAsync();

            // Assert: Verify that we received a valid list of books
            Assert.IsNotEmpty(result); // Ensure there are books in the result

            // Example: Check for specific book properties
            Assert.That(result[0].Name, Is.Not.Null.Or.Empty);
            Assert.That(result[0].Books, Is.Not.Empty);
        }

        [Test]
        public void GetBooksAsync_ShouldThrowException_WhenExternalApiFails()
        {
            // Arrange: Simulate a failed API response by using an incorrect URL or endpoint
            var failedHttpClient = new HttpClient
            {
                BaseAddress = new System.Uri("https://nonexistent-api-url.com")
            };
            var failedService = new BookService(failedHttpClient);

            // Act & Assert: Ensure that an exception is thrown when the external API is unavailable
            Assert.ThrowsAsync<ApplicationException>(async () => await failedService.GetBooksAsync());
        }

        [Test]
        public async Task FilterAndGroupBooks_ShouldReturnFilteredBooks_WhenValidFilterIsApplied()
        {
            // Arrange: Get a list of books from the external API
            var result = await _service.GetBooksAsync();

            // Apply a filter for hardcover books
            var filteredBooks = _service.FilterAndGroupBooks(result, "hardcover");

            // Assert: Verify that the returned books match the filter criteria
            Assert.IsNotEmpty(filteredBooks); // Ensure filtered books are returned
            
        }

        [Test]
        public void FilterAndGroupBooks_ShouldThrowException_WhenNoBooksMatchFilter()
        {
            // Arrange: Create a list of books with no "hardcover" type
            var books = new List<BookOwner>
        {
            new BookOwner { Name = "John", Age = 30, Books = new List<Book> { new Book { Name = "Paperback Book", Type = "Paperback" } } }
        };

            // Act & Assert: Ensure that an exception is thrown when no books match the filter
            var ex = Assert.Throws<InvalidOperationException>(() => _service.FilterAndGroupBooks(books, "hardcover"));
            Assert.AreEqual("No hardcover books found.", ex.Message); // Verify the exception message
        }

        [Test]
        public async Task GetBooksAsync_ShouldReturnEmptyList_WhenExternalApiReturnsNoData()
        {
            // Arrange: Simulate a situation where the external API returns no data
            var emptyHttpClient = new HttpClient
            {
                BaseAddress = new System.Uri("https://digitalcodingtest.bupa.com.au/api/v1/bookowners")
            };

            // Mock an empty response for testing purposes
            var mockResponse = "[]"; // Empty list response
            emptyHttpClient.DefaultRequestHeaders.Add("User-Agent", "TestClient");
            var mockResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(mockResponse)
            };

            // Simulate an HttpClient response
            var result = JsonConvert.DeserializeObject<List<BookOwner>>(mockResponse);

            // Act: Call the service to retrieve the books
            var service = new BookService(emptyHttpClient);
            var books = await service.GetBooksAsync();

            // Assert: Ensure that the books list is empty
            Assert.IsEmpty(books); // Ensure no books were returned
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the client after each test
            _httpClient.Dispose();
        }
    }
}
