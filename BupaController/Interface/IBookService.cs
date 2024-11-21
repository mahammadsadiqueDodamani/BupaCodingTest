using Bupa.Models;

namespace Bupa.Interface
{
    public interface IBookService
    {
        Task<List<BookOwner>> GetBooksAsync();
    }
}
