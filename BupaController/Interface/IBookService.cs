using Bupa.Models;

namespace Bupa.Interface
{
    /// <summary>
    /// Fetch the extenal API and provide the list of result present in the API.
    /// </summary>
    public interface IBookService
    {
        Task<List<BookOwner>> GetBooksAsync();
    }
}
