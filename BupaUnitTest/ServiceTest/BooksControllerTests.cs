using Bupa.Controllers;
using Bupa.Interface;
using Bupa.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BupaTest.ServiceTest
{
    [TestFixture]
    public class BooksControllerTests
    {
        private Mock<IBookService> _mockBookService;
        private BooksController _controller;

        [SetUp]
        public void SetUp()
        {
            // Create a mock of IBookService
            _mockBookService = new Mock<IBookService>();
            _controller = new BooksController(_mockBookService.Object);
        }

        [Test]
        public async Task GetBooks_ReturnsOk_WhenBooksFound()
        {
            // Arrange: Mock the service to return a list of books
            var books = new List<BookOwner> { new BookOwner { Name = "John", Age = 30, Books = new List<Book> { new Book { Name = "Hardcover Book", Type = "Hardcover" } } } };
            _mockBookService.Setup(service => service.GetBooksAsync()).ReturnsAsync(books);
            _mockBookService.Setup(service => service.FilterAndGroupBooks(books, "all")).Returns(new List<dynamic> { new { BookName = "Hardcover Book" } });

            // Act: Call the GetBooks method on the controller
            var result = await _controller.GetBooks("all");

            // Assert: Verify that the result is an OkObjectResult
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var actionResult = result as OkObjectResult;
            var returnValue = actionResult?.Value as IEnumerable<dynamic>;
            Assert.IsNotEmpty(returnValue); // Ensure there's data in the response
        }

        [Test]
        public async Task GetBooks_ReturnsNotFound_WhenNoBooksFound()
        {
            // Arrange: Mock the service to return an empty list
            _mockBookService.Setup(service => service.GetBooksAsync()).ReturnsAsync(new List<BookOwner>());

            // Act: Call the GetBooks method on the controller
            var result = await _controller.GetBooks("all");

            // Assert: Verify that the result is a NotFoundObjectResult
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var actionResult = result as NotFoundObjectResult;
            var returnValue = actionResult?.Value as dynamic;
            Assert.AreEqual("No books found.", returnValue.message); // Verify the message returned
        }

        [Test]
        public async Task GetBooks_ReturnsBadRequest_WhenNoBooksMatchFilter()
        {
            // Arrange: Mock the service to return books that do not match the filter
            var books = new List<BookOwner> { new BookOwner { Name = "John", Age = 30, Books = new List<Book> { new Book { Name = "Paperback Book", Type = "Paperback" } } } };
            _mockBookService.Setup(service => service.GetBooksAsync()).ReturnsAsync(books);
            _mockBookService.Setup(service => service.FilterAndGroupBooks(books, "hardcover")).Returns(new List<dynamic>());

            // Act: Call the GetBooks method on the controller with a filter
            var result = await _controller.GetBooks("hardcover");

            // Assert: Verify that the result is a BadRequestObjectResult
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var actionResult = result as BadRequestObjectResult;
            var returnValue = actionResult?.Value as dynamic;
            Assert.AreEqual("No books found with the filter 'hardcover'.", returnValue.message); // Verify the message returned
        }
    }

}
