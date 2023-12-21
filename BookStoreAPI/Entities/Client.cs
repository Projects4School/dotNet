namespace BookStoreAPI.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string FirstName { get; set; }
        public required string Address { get; set; }
    }
}