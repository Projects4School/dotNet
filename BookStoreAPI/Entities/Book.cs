namespace BookStoreAPI.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; init; }
        public string? Author { get; set; }

        public string Abstract { get; set; } = string.Empty;
    }
}