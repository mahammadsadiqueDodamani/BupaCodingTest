using Bupa.Interface;
using Bupa.Models;
using Bupa.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bupa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] string filter = "all")
        {
            var owners = await _bookService.GetBooksAsync();

            if (filter == "hardcover")
            {
                owners = owners.Select(owner => new BookOwner
                {
                    Name = owner.Name,
                    Age = owner.Age,
                    Books = owner.Books.Where(book => book.Type == "Hardcover").ToList()
                }).ToList();
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
