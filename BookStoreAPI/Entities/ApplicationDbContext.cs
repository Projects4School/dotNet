using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Entities;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var currentDir = Directory.GetCurrentDirectory();

        var dbPath = Path.Combine(currentDir, "Bookstore.db");
        Console.WriteLine($"dbPath: {dbPath}");
        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }

    public DbSet<Book> Books { get; set; } = default!;
}