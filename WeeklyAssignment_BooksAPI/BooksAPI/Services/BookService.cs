using BooksAPI.Interfaces;
using BooksAPI.Models;
using BooksAPI.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BooksAPI.Services;

public class BookService : IBookService
{
    private readonly string _connectionString;
    private readonly PaginationOptions _paginationOptions;

    public BookService(IConfiguration configuration, IOptions<PaginationOptions> paginationOptions)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        _paginationOptions = paginationOptions.Value;
    }

    public async Task<IEnumerable<Book>> GetAllAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        var books = new List<Book>();
        int actualPageSize = pageSize > 0 ? Math.Min(pageSize, _paginationOptions.MaxPageSize) : _paginationOptions.DefaultPageSize;
        int actualPageNumber = pageNumber > 0 ? pageNumber : 1;
        int offset = (actualPageNumber - 1) * actualPageSize;

        using (var connection = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM Books WHERE 1=1";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sql += " AND (Title LIKE @search OR Author LIKE @search)";
            }
            sql += " ORDER BY Id OFFSET @offset ROWS FETCH NEXT @size ROWS ONLY";

            var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@offset", offset);
            command.Parameters.AddWithValue("@size", actualPageSize);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                command.Parameters.AddWithValue("@search", $"%{searchTerm}%");
            }

            await connection.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    books.Add(new Book
                    {
                        Id = reader.GetInt32("Id"),
                        Title = reader.GetString("Title"),
                        Author = reader.GetString("Author"),
                        CategoryId = reader.IsDBNull("CategoryId") ? 0 : reader.GetInt32("CategoryId")
                    });
                }
            }
        }
        return books;
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand("SELECT * FROM Books WHERE Id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Book
                    {
                        Id = reader.GetInt32("Id"),
                        Title = reader.GetString("Title"),
                        Author = reader.GetString("Author"),
                        CategoryId = reader.IsDBNull("CategoryId") ? 0 : reader.GetInt32("CategoryId")
                    };
                }
            }
        }
        return null;
    }

    public async Task<Book> CreateAsync(Book book)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand(
                "INSERT INTO Books (Title, Author, CategoryId) OUTPUT INSERTED.Id VALUES (@title, @author, @catId)", 
                connection);
            command.Parameters.AddWithValue("@title", book.Title);
            command.Parameters.AddWithValue("@author", book.Author);
            command.Parameters.AddWithValue("@catId", book.CategoryId);

            await connection.OpenAsync();
            book.Id = (int)await command.ExecuteScalarAsync();
        }
        return book;
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand(
                "UPDATE Books SET Title = @title, Author = @author, CategoryId = @catId WHERE Id = @id", 
                connection);
            command.Parameters.AddWithValue("@id", book.Id);
            command.Parameters.AddWithValue("@title", book.Title);
            command.Parameters.AddWithValue("@author", book.Author);
            command.Parameters.AddWithValue("@catId", book.CategoryId);

            await connection.OpenAsync();
            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var command = new SqlCommand("DELETE FROM Books WHERE Id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync();
            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
}
