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
    public DbSet<Author> Authors { get; set; } = default!;
    public DbSet<Genre> Genres { get; set; } = default!;
    public DbSet<Publisher> Publishers { get; set; } = default!;
}