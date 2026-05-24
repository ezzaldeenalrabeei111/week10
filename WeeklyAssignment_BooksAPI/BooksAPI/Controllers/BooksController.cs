using BooksAPI.Interfaces;
using BooksAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooksAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? search = null)
    {
        var books = await _bookService.GetAllAsync(page, size, search);
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book == null) return NotFound();
        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Book book)
    {
        var created = await _bookService.CreateAsync(book);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Book book)
    {
        if (id != book.Id) return BadRequest();
        var success = await _bookService.UpdateAsync(book);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _bookService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
