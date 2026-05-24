namespace BooksAPI.Configuration;

public class PaginationOptions
{
    public int DefaultPageSize { get; set; } = 10;
    public int MaxPageSize { get; set; } = 50;
}

public class AppSettings
{
    public string ApiName { get; set; } = "Books API";
    public string Version { get; set; } = "1.0.0";
}
