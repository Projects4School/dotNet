using AutoMapper;
using BookStoreAPI.Entities;
using BookStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookStoreAPI.Controllers; 

[ApiController]
[Route("api/[controller]")]

public class GenreController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GenreController(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<Genre>))]
    [ProducesResponseType(400)]
    public async Task<ActionResult<List<Genre>>> GetGenres()
    {
        var genres = await _dbContext.Genres.ToListAsync();
        var genresDto = new List<GenreDto>();

        foreach (var genre in genres)
        {
            genresDto.Add(_mapper.Map<GenreDto>(genre));
        }

        return Ok(genresDto);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(GenreDto))]
    [ProducesResponseType(404)]
    public async Task<ActionResult<GenreDto>> GetGenre(int id)
    {
        Genre? existingGenre = await _dbContext.Genres.FirstOrDefaultAsync(b => b.Id == id);
        if (existingGenre is null)
        {
            return NotFound();
        }
        else
        {
            return Ok(_mapper.Map<GenreDto>(existingGenre));
        }
    }

    //[Authorize]
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Genre))]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Genre>> PostGenre([FromBody] Genre genre)
    {
        if (genre is null)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Genre? addedGenre = await _dbContext.Genres.FirstOrDefaultAsync(b => b.Name == genre.Name);
        if (addedGenre is not null)
        {
            return BadRequest("Genre already exists");
        }
        else
        {
            try
            {
                await _dbContext.Genres.AddAsync(genre);
                await _dbContext.SaveChangesAsync();
                return Created("api/genre", genre);
            }
            catch (DbUpdateException exception)
            {
                return BadRequest(exception.InnerException?.Message);
            }
        }
    }

    //[Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(200, Type = typeof(Genre))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Genre>> PutGenre(int id, [FromBody] Genre genre)
    {
        if (genre is null || id != genre.Id)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var genreToUpdate = await _dbContext.Genres.FirstOrDefaultAsync(b => b.Id == id);
        if (genreToUpdate is null)
        {
            return NotFound();
        }
        else
        {
            genreToUpdate.Name = genre.Name;

            try
            {
                _dbContext.Genres.Entry(genreToUpdate).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(genreToUpdate);
            }
            catch (DbUpdateException exception)
            {
                return BadRequest(exception.InnerException?.Message);
            }
        }
    }

    //[Aurthorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(200, Type = typeof(Genre))]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Genre>> DeleteGenre(int id)
    {
        Genre? existingGenre = await _dbContext.Genres.FirstOrDefaultAsync(b => b.Id == id);
        if (existingGenre is null)
        {
            return NotFound();
        }
        else
        {
            try
            {
                _dbContext.Genres.Remove(existingGenre);
                await _dbContext.SaveChangesAsync();
                return Ok(existingGenre);
            }
            catch (DbUpdateException exception)
            {
                return BadRequest(exception.InnerException?.Message);
            }
        }
    }
}
