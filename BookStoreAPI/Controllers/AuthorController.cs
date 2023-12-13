using BookStoreAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookStoreAPI.Controllers; 

[ApiController]
[Route("api/[controller]")]

public class AuthorController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public AuthorController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<Author>))]
    [ProducesResponseType(400)]
    public async Task<ActionResult<List<Author>>> GetAuthors()
    {
        return await _dbContext.Authors.ToListAsync();
    }
}
