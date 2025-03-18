using BookAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookAPI.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetBooks();
        Task<Book> GetBookById(int id);
        Task<IEnumerable<Book>> GetBooksByAuthor(string authorName);
        Task<IEnumerable<Book>> GetBooksByCategory(string category);
        Task<Book> CreateBook(Book book);
        Task UpdateBook(Book book);
        Task DeleteBook(int id);
        Task<bool> BookExists(int id);
    }
}
