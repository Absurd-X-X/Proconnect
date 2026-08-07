namespace Application.Contract.Settings
{
    public class LoginResponse
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = default!;

        public string Role { get; set; } = default!;

        public string ProfileId { get; set; } = default!;

        public string UserName { get; set; } = default!;
    }
}
