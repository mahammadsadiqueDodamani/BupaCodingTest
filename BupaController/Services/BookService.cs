using System.Text.Json;
using Bupa.Interface;
using Bupa.Models;
using Newtonsoft.Json;

namespace Bupa.Services
{
    public class BookService:IBookService
    {
        private readonly HttpClient _httpClient;

        public BookService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BookOwner>> GetBooksAsync()
        {
            var response = await _httpClient.GetStringAsync("https://digitalcodingtest.bupa.com.au/api/v1/bookowners");
            return JsonConvert.DeserializeObject<List<BookOwner>>(response);
        }
    }
}
