using AutoMapper;
using BookStoreAPI.Entities;
using BookStoreAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookStoreAPI.Controllers; 

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public BookController(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    //[Authorize]
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<BookDto>))]
    [ProducesResponseType(400)]
    public async Task<ActionResult<List<BookDto>>> GetBooks()
    {
        var books = await _dbContext.Books.Include(a => a.Author).ToListAsync();
        var booksDto = new List<BookDto>();

        foreach (var book in books)
        {
            booksDto.Add(_mapper.Map<BookDto>(book));
        }

        return Ok(booksDto);
    }

    //[Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(BookDto))]
    [ProducesResponseType(404)]
    public async Task<ActionResult<BookDto>> GetBook(int id)
    {
        Book? existingBook = await _dbContext.Books
        .Include(a => a.Author)
        .FirstOrDefaultAsync(b => b.Id == id);
        if (existingBook is null)
        {
            return NotFound();
        }
        else
        {
            return Ok(_mapper.Map<BookDto>(existingBook));
        }
    }

    //[Authorize]
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Book))]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Book>> PostBook([FromBody] Book book)
    {
        if (book is null)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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

    //[Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(200, Type = typeof(Book))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Book>> PutBook(int id, [FromBody] Book book)
    {
        if (book is null || id != book.Id)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var bookToUpdate = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (bookToUpdate is null)
        {
            return NotFound();
        }
        else
        {
            bookToUpdate.Title = book.Title;
            bookToUpdate.Author = book.Author;
            bookToUpdate.Abstract = book.Abstract;

            try
            {
                _dbContext.Books.Entry(bookToUpdate).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(bookToUpdate);
            }
            catch (DbUpdateException exception)
            {
                return BadRequest(exception.InnerException?.Message);
            }
        }
    }

    //[Authorize]
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