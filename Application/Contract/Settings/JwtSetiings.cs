namespace Application.Contract.Settings
{
    public class JwtSetiings
    {
        public string SecretKey { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int Expiry { get; set; } 
    }
}
