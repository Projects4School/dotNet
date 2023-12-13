using System.ComponentModel.DataAnnotations;
using BookStoreAPI.Entities;

namespace BookStoreAPI.Models;

public class BookDto
{
    public string Title { get; init; } = default!;

    public Author? Author { get; set; }
}