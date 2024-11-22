using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bupa.Models;
using Bupa.Services;
using Moq;

namespace BupaTest.ServiceTest
{
    [TestFixture]
    public class BookServiceTest
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private HttpClient _httpClient;
        private BookService _bookService;

        [SetUp]
        public void SetUp()
        {
            // Setup mocks and dependencies
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClient = new HttpClient();
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(_httpClient);

            _bookService = new BookService(_httpClient);
        }

        [Test]
        public void FilterAndGroupBooks_WhenCalledWithHardcoverFilter_ReturnsOnlyHardcoverBooks()
        {
            // Arrange: Prepare test data
            var owners = new List<BookOwner>
            {
                new BookOwner
                {
                    Name = "John",
                    Age = 25,
                    Books = new List<Book>
                    {
                        new Book { Name = "Hardcover Book 1", Type = "Hardcover" },
                        new Book { Name = "Paperback Book 1", Type = "Paperback" }
                    }
                },
                new BookOwner
                {
                    Name = "Sarah",
                    Age = 10,
                    Books = new List<Book>
                    {
                        new Book { Name = "Hardcover Book 2", Type = "Hardcover" }
                    }
                }
            };

            var filter = "hardcover";

            // Act: Call the method under test
            var result = _bookService.FilterAndGroupBooks(owners, filter);

            // Assert: Verify the expected outcome
            Assert.AreEqual(2, result.Count);  // Two hardcover books should be returned
        }

        [Test]
        public void FilterAndGroupBooks_WhenNoBooksFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var owners = new List<BookOwner>
            {
                new BookOwner { Name = "John", Age = 25, Books = new List<Book>() }
            };
            var filter = "hardcover";

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _bookService.FilterAndGroupBooks(owners, filter));
            Assert.AreEqual("No hardcover books found.", ex.Message);
        }

        [Test]
        public void FilterAndGroupBooks_ReturnsFilteredAndGroupedBooks_WhenValidDataIsPassed()
        {
            // Arrange: Mock book owners
            var owners = new List<BookOwner>
            {
                new BookOwner
                {
                    Name = "John",
                    Age = 30,
                    Books = new List<Book>
                    {
                        new Book { Name = "Hardcover Book 1", Type = "Hardcover" },
                        new Book { Name = "Paperback Book 1", Type = "Paperback" }
                    }
                },
                new BookOwner
                {
                    Name = "Jane",
                    Age = 10,
                    Books = new List<Book>
                    {
                        new Book { Name = "Hardcover Book 2", Type = "Hardcover" }
                    }
                }
            };

            // Act
            var result = _bookService.FilterAndGroupBooks(owners, "hardcover");

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count); // Expect two books in the result
        }
        [TearDown]
        public void TearDown()
        {
            // Clean up the client after each test
            _httpClient.Dispose();
        }
    }
}
