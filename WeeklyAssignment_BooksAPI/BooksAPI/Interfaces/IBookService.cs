using BooksAPI.Models;

namespace BooksAPI.Interfaces;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAllAsync(int pageNumber, int pageSize, string? searchTerm);
    Task<Book?> GetByIdAsync(int id);
    Task<Book> CreateAsync(Book book);
    Task<bool> UpdateAsync(Book book);
    Task<bool> DeleteAsync(int id);
}

public interface ICategoryService
{
    Task<IEnumerable<object>> GetAllWithBookCountAsync();
    Task<object?> GetByIdWithBooksAsync(int id);
}
