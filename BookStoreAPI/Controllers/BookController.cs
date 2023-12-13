using BookStoreAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookStoreAPI.Controllers; 

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{

    private readonly ApplicationDbContext _dbContext;

    public BookController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<Book>>> GetBooks()
    {
        return await _dbContext.Books.ToListAsync();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Book))]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        Book? existingBook = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (existingBook is null)
        {
            return NotFound();
        }
        else
        {
            return Ok(existingBook);
        }
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Book))]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Book>> PostBook([FromBody] Book book)
    {
        if (book is null)
        {
            return BadRequest();
        }
        Book? addedBook = await _dbContext.Books.FirstOrDefaultAsync(b => b.Title == book.Title);
        if (addedBook is not null)
        {
            return BadRequest("Book already exists");
        }
        else
        {
            try
            {
                await _dbContext.Books.AddAsync(book);
                await _dbContext.SaveChangesAsync();
                return Created("api/book", book);
            }
            catch (DbUpdateException exception)
            {
                return BadRequest(exception.InnerException?.Message);
            }
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(200, Type = typeof(Book))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Book>> PutBook(int id, [FromBody] Book book)
    {
        // we check if the parameter is null
        if (book is null || id != book.Id)
        {
            return BadRequest();
        }
        // we check if the book already exists
        Book? existingBook = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (existingBook is null)
        {
            return NotFound();
        }
        else
        {
            existingBook = new Book
            {
                Title = book.Title,
                Author = book.Author,
                Abstract = book.Abstract
            };

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok(existingBook);
            }
            catch (DbUpdateException exception)
            {
                return BadRequest(exception.InnerException?.Message);
            }
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(200, Type = typeof(Book))]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Book>> DeleteBook(int id)
    {
        Book? existingBook = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (existingBook is null)
        {
            return NotFound();
        }
        else
        {
            try
            {
                _dbContext.Books.Remove(existingBook);
                await _dbContext.SaveChangesAsync();
                return Ok(existingBook);
            }
            catch (DbUpdateException exception)
            {
                return BadRequest(exception.InnerException?.Message);
            }
        }
    }
}