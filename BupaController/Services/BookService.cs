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
            var response = await _httpClient.GetStringAsync("https://digitalcodingtest.bupa.com.au/api/v1/bookowners");
            return JsonConvert.DeserializeObject<List<BookOwner>>(response);
        }
    }
}
