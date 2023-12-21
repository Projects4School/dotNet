using AutoMapper;
using BookStoreAPI.Entities;
using BookStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookStoreAPI.Controllers; 

[ApiController]
[Route("api/[controller]")]

public class PublisherController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public PublisherController(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<Publisher>))]
    [ProducesResponseType(400)]
    public async Task<ActionResult<List<Publisher>>> GetPublishers()
    {
        var publishers = await _dbContext.Publishers.ToListAsync();
        var publisherDto = new List<PublisherDto>();

        foreach (var publisher in publishers)
        {
            publisherDto.Add(_mapper.Map<PublisherDto>(publisher));
        }

        return Ok(publisherDto);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(PublisherDto))]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PublisherDto>> GetPublisher(int id)
    {
        Publisher? existingPublisher = await _dbContext.Publishers.FirstOrDefaultAsync(b => b.Id == id);
        if (existingPublisher is null)
        {
            return NotFound();
        }
        else
        {
            return Ok(_mapper.Map<PublisherDto>(existingPublisher));
        }
    }

    //[Authorize]
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Publisher))]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Publisher>> PostPublisher([FromBody] Publisher publisher)
    {
        if (publisher is null)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Publisher? addedPublisher = await _dbContext.Publishers.FirstOrDefaultAsync(b => b.Name == publisher.Name && b.Country == publisher.Country);
        if (addedPublisher is not null)
        {
            return BadRequest("Publisher already exists");
        }
        else
        {
            try
            {
                await _dbContext.Publishers.AddAsync(publisher);
                await _dbContext.SaveChangesAsync();
                return Created("api/publisher", publisher);
            }
            catch (DbUpdateException exception)
            {
                return BadRequest(exception.InnerException?.Message);
            }
        }
    }

    //[Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(200, Type = typeof(Publisher))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Publisher>> PutPublisher(int id, [FromBody] Publisher publisher)
    {
        if (publisher is null || id != publisher.Id)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var publisherToUpdate = await _dbContext.Publishers.FirstOrDefaultAsync(b => b.Id == id);
        if (publisherToUpdate is null)
        {
            return NotFound();
        }
        else
        {
            publisherToUpdate.Name = publisher.Name;

            try
            {
                _dbContext.Publishers.Entry(publisherToUpdate).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(publisherToUpdate);
            }
            catch (DbUpdateException exception)
            {
                return BadRequest(exception.InnerException?.Message);
            }
        }
    }

    //[Aurthorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(200, Type = typeof(Publisher))]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Publisher>> DeletePublisher(int id)
    {
        Publisher? existingPublisher = await _dbContext.Publishers.FirstOrDefaultAsync(b => b.Id == id);
        if (existingPublisher is null)
        {
            return NotFound();
        }
        else
        {
            try
            {
                _dbContext.Publishers.Remove(existingPublisher);
                await _dbContext.SaveChangesAsync();
                return Ok(existingPublisher);
            }
            catch (DbUpdateException exception)
            {
                return BadRequest(exception.InnerException?.Message);
            }
        }
    }
}
