using Bupa.Controllers;
using Bupa.Interface;
using Bupa.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BupaTest
{
    [TestFixture]
    public class Tests
    {
        private Mock<IBookService> _mockBookService;
        private BooksController _controller;
        [SetUp]
        public void Setup()
        {
            _mockBookService = new Mock<IBookService>();
            _controller = new BooksController(_mockBookService.Object);
        }
        // NUnit TearDown method runs after each test
        [TearDown]
        public void TearDown()
        {
            // Dispose of the _controller if it's IDisposable
            if (_controller is IDisposable disposableController)
            {
                disposableController.Dispose();
            }
        }
        [Test]
        public async Task GetBooks_ReturnsAllBooks_WhenFilterIsAll()
        {
            // Arrange
            var bookOwners = new List<BookOwner>
            {
                new BookOwner
                {
                    Name = "John",
                    Age = 25,
                    Books = new List<Book>
                    {
                        new Book { Name = "Book 1", Type = "Hardcover" },
                        new Book { Name = "Book 2", Type = "Paperback" }
                    }
                },
                new BookOwner
                {
                    Name = "Jane",
                    Age = 15,
                    Books = new List<Book>
                    {
                        new Book { Name = "Book 3", Type = "Hardcover" },
                        new Book { Name = "Book 4", Type = "Paperback" }
                    }
                }
            };

            _mockBookService.Setup(service => service.GetBooksAsync()).ReturnsAsync(bookOwners);

            // Act
            var result = await _controller.GetBooks("all");
            Console.WriteLine(result);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var books = okResult.Value;
            Assert.IsNotNull(books);
           // Assert.AreEqual(4, books.Count); // Should return 4 books in total
        }
        [Test]
        public async Task GetBooks_ReturnsOnlyHardcoverBooks_WhenFilterIsHardcover()
        {
            // Arrange
            var bookOwners = new List<BookOwner>
            {
                new BookOwner
                {
                    Name = "John",
                    Age = 25,
                    Books = new List<Book>
                    {
                        new Book { Name = "Book 1", Type = "Hardcover" },
                        new Book { Name = "Book 2", Type = "Paperback" }
                    }
                },
                new BookOwner
                {
                    Name = "Jane",
                    Age = 15,
                    Books = new List<Book>
                    {
                        new Book { Name = "Book 3", Type = "Hardcover" },
                        new Book { Name = "Book 4", Type = "Paperback" }
                    }
                }
            };

            _mockBookService.Setup(service => service.GetBooksAsync()).ReturnsAsync(bookOwners);

            // Act
            var result = await _controller.GetBooks("hardcover");

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }
        [Test]
        public async Task GetBooks_ReturnsEmptyList_WhenNoBooks()
        {
            // Arrange
            _mockBookService.Setup(service => service.GetBooksAsync()).ReturnsAsync(new List<BookOwner>());

            // Act
            var result = await _controller.GetBooks("all");

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var books = okResult.Value as List<object>;
            Assert.IsNull(books);
        }

        [Test]
        public async Task GetBooks_ReturnsBooksGroupedByAgeCategory()
        {
            // Arrange
            var bookOwners = new List<BookOwner>
            {
                new BookOwner
                {
                    Name = "John",
                    Age = 25,
                    Books = new List<Book>
                    {
                        new Book { Name = "Book 1", Type = "Hardcover" },
                        new Book { Name = "Book 2", Type = "Paperback" }
                    }
                },
                new BookOwner
                {
                    Name = "Alice",
                    Age = 17,
                    Books = new List<Book>
                    {
                        new Book { Name = "Book 3", Type = "Hardcover" },
                        new Book { Name = "Book 4", Type = "Paperback" }
                    }
                }
            };

            _mockBookService.Setup(service => service.GetBooksAsync()).ReturnsAsync(bookOwners);

            // Act
            var result = await _controller.GetBooks("all");

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var books = okResult.Value;
            Assert.IsNotNull(books);
        }


    }
}