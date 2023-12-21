namespace BookStoreAPI.Models
{
    public class AuthorCreateRequestDto
    {
        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;
    }
}