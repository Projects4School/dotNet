using AutoMapper;
using BookStoreAPI.Entities;
using BookStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookStoreAPI.Controllers; 

[ApiController]
[Route("api/[controller]")]

public class AuthorController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public AuthorController(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<Author>))]
    [ProducesResponseType(400)]
    public async Task<ActionResult<List<Author>>> GetAuthors()
    {
        var authors = await _dbContext.Authors.ToListAsync();
        var authorsDto = new List<AuthorDto>();

        foreach (var author in authors)
        {
            authorsDto.Add(_mapper.Map<AuthorDto>(author));
        }

        return Ok(authorsDto);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(AuthorDto))]
    [ProducesResponseType(404)]
    public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
    {
        Author? existingAuthor = await _dbContext.Authors.FirstOrDefaultAsync(b => b.Id == id);
        if (existingAuthor is null)
        {
            return NotFound();
        }
        else
        {
            return Ok(_mapper.Map<AuthorDto>(existingAuthor));
        }
    }

    //[Authorize]
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Author))]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Author>> PostAuthor([FromBody] Author author)
    {
        if (author is null)
        {
            return BadRequest();
        }
        Author? addedAuthor = await _dbContext.Authors.FirstOrDefaultAsync(b => b.FirstName == author.FirstName && b.LastName == author.LastName);
        if (addedAuthor is not null)
        {
            return BadRequest("Author already exists");
        }
        else
        {
            try
            {
                await _dbContext.Authors.AddAsync(author);
                await _dbContext.SaveChangesAsync();
                return Created("api/author", author);
            }
            catch (DbUpdateException exception)
            {
                return BadRequest(exception.InnerException?.Message);
            }
        }
    }

    //[Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(200, Type = typeof(Author))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Author>> PutBook(int id, [FromBody] Author author)
    {
        if (author is null || id != author.Id)
        {
            return BadRequest();
        }
        var authorToUpdate = await _dbContext.Authors.FirstOrDefaultAsync(b => b.Id == id);
        if (authorToUpdate is null)
        {
            return NotFound();
        }
        else
        {
            authorToUpdate.FirstName = author.FirstName;
            authorToUpdate.LastName = author.LastName;  

            try
            {
                _dbContext.Authors.Entry(authorToUpdate).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(authorToUpdate);
            }
            catch (DbUpdateException exception)
            {
                return BadRequest(exception.InnerException?.Message);
            }
        }
    }

    [HttpPost("validationTest")]
    public ActionResult ValidationTest([FromBody] AuthorDto author)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return Ok();
    }

    //[Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(200, Type = typeof(Author))]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Author>> DeleteBook(int id)
    {
        Author? existingAuthor = await _dbContext.Authors.FirstOrDefaultAsync(b => b.Id == id);
        if (existingAuthor is null)
        {
            return NotFound();
        }
        else
        {
            try
            {
                _dbContext.Authors.Remove(existingAuthor);
                await _dbContext.SaveChangesAsync();
                return Ok(existingAuthor);
            }
            catch (DbUpdateException exception)
            {
                return BadRequest(exception.InnerException?.Message);
            }
        }
    }
}
