using Moq;
using Bupa.Controllers;
using Bupa.Interface;

namespace BookControllerTest
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

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}