using System.Text.Json;
using Bupa.Interface;
using Bupa.Models;
using Newtonsoft.Json;

namespace Bupa.Services
{
    /// <summary>
    /// Service to fetch the External API
    /// </summary>
    public class BookService:IBookService
    {
        //DI for fetching the API
        private readonly HttpClient _httpClient;

        public BookService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get the book details.
        /// </summary>
        /// <returns>The list of book present in the external api</returns>
        public async Task<List<BookOwner>> GetBooksAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("https://digitalcodingtest.bupa.com.au/api/v1/bookowners");

                if (string.IsNullOrEmpty(response))
                {
                    throw new Exception("No response received from external API.");
                }

                return JsonConvert.DeserializeObject<List<BookOwner>>(response);
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow
                // Logger.LogError(ex, "Failed to fetch books");
                throw new ApplicationException("An error occurred while fetching book data.", ex);
            }
        }

        /// <summary>
        /// Filter books by type and group them by the owner age category.
        /// </summary>
        /// <param name="owners">The list of book owners.</param>
        /// <param name="filter">The filter criteria (e.g., "hardcover").</param>
        /// <returns>A list of books grouped by owner age category.</returns>
        public List<dynamic> FilterAndGroupBooks(List<BookOwner> owners, string filter)
        {
            if (owners == null || !owners.Any())
            {
                throw new InvalidOperationException("No books found.");
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

            // If the filtered owners have no books after filtering, return an empty list
            if (!owners.Any(owner => owner.Books.Any()))
            {
                throw new InvalidOperationException("No hardcover books found.");
            }

            var result= owners
                .SelectMany(owner => owner.Books.Select(book => new
                {
                    AgeCategory = owner.Age >= 18 ? "Adults" : "Children",
                    BookName = book.Name,
                    OwnerName = owner.Name,
                    OwnerAge = owner.Age
                }))
                .OrderBy(book => book.BookName) // Sort alphabetically by book name
                .ToList();
            return result.Cast<dynamic>().ToList();
        }
    }
}
