using System.ComponentModel.DataAnnotations;
using BookStoreAPI.Entities;

namespace BookStoreAPI.Models;

public class AuthorDto
{
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
}