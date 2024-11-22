namespace Bupa.Models
{
    /// <summary>
    /// owner property details
    /// </summary>
    public class BookOwner
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<Book> Books { get; set; }
    }
}
