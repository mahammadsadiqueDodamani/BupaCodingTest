using Bupa.Constants;
using Bupa.Interface;
using Bupa.Models;
using Newtonsoft.Json;

namespace Bupa.Services
{
    /// <summary>
    /// Service to fetch the External API
    /// </summary>
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;

        public BookService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get the book details.
        /// </summary>
        /// <returns>The list of books present in the external API</returns>
        public async Task<List<BookOwner>> GetBooksAsync()
        {
            var response = await _httpClient.GetStringAsync(ApiConstants.BaseUrl);

            if (string.IsNullOrEmpty(response))
            {
                throw new ApplicationException(ApiConstants.ErrorMessage);
            }

            return JsonConvert.DeserializeObject<List<BookOwner>>(response);
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

            if (filter == BookFilterConstants.Hardcover)
            {
                owners = owners.Select(owner => new BookOwner
                {
                    Name = owner.Name,
                    Age = owner.Age,
                    Books = owner.Books.Where(book => book.Type == "Hardcover").ToList()
                }).ToList();
            }

            if (!owners.Any(owner => owner.Books.Any()))
            {
                throw new InvalidOperationException("No hardcover books found.");
            }

            var result = owners
                .SelectMany(owner => owner.Books.Select(book => new
                {
                    AgeCategory = owner.Age >= 18 ? "Adults" : "Children",
                    BookName = book.Name,
                    OwnerName = owner.Name,
                    OwnerAge = owner.Age
                }))
                .OrderBy(book => book.BookName)
                .ToList();

            return result.Cast<dynamic>().ToList();
        }
    }

}
