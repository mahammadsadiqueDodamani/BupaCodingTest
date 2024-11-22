using Bupa.Interface; 
using Bupa.Models;
using Bupa.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bupa.Controllers
{
    /// <summary>
    /// API to fetch the records.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// GetBooks from the service/db to display the books available.
        /// </summary>
        /// <param name="filter">Used to filter the books based on parameter passed to the method</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] string filter = "all")
        {
            var owners = await _bookService.GetBooksAsync();

            // Check if the service returned a null or empty list
            if (owners == null || !owners.Any())
            {
                // Return a 404 Not Found or 400 Bad Request if no data is found
                return NotFound(new { message = "No books found." });
            }

            if (filter == "hardcover")
            {
                owners = owners.Select(owner => new BookOwner
                {
                    Name = owner.Name,
                    Age = owner.Age,
                    Books = owner.Books.Where(book => book.Type == "Hardcover").ToList()
                }).ToList();
            }

            // If the filtered owners have no books after filtering, return a specific message
            if (!owners.Any(owner => owner.Books.Any()))
            {
                return NotFound(new { message = "No hardcover books found." });
            }

            var groupedBooks = owners
            .SelectMany(owner => owner.Books.Select(book => new
            {
                AgeCategory = owner.Age >= 18 ? "Adults" : "Children",
                BookName = book.Name,
                OwnerName = owner.Name,
                OwnerAge = owner.Age,
                
            }))
            .OrderBy(book => book.BookName) // Sort alphabetically by book name
            .ToList();
            return Ok(groupedBooks);
        }
    }
}
