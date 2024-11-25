using Bupa.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Bupa.Controllers
{
    /// <summary>
    /// API controller to fetch and display books.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Get books from the service, optionally filtered by type.
        /// </summary>
        /// <param name="filter">Filter books by type (e.g., "hardcover"). Defaults to "all".</param>
        /// <returns>A list of filtered and grouped books.</returns>
        /// <response code="200">Returns the list of filtered books</response>
        /// <response code="400">If no books found after filtering</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<dynamic>), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetBooks([FromQuery] string filter = "all")
        {
            var owners = await _bookService.GetBooksAsync();
            if (owners == null || !owners.Any())
            {
                // No books found, return 404 Not Found.
                return NotFound(new { message = "No books found." });
            }

            var groupedBooks = _bookService.FilterAndGroupBooks(owners, filter);

            if (groupedBooks == null || !groupedBooks.Any())
            {
                // If no books match the filter, return 400 Bad Request.
                return BadRequest(new { message = $"No books found with the filter '{filter}'." });
            }

            return Ok(groupedBooks);
        }
    }
}
