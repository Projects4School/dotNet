namespace BookStoreAPI.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
    }
}