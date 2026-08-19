using Application.Contract.Settings;
using Application.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Infrastructure.Authentication
{
    public class TokenService : ITokenServices
    {
        private readonly JwtSetiings _settings;

        public TokenService(IOptions<JwtSetiings> settings)
        {
            _settings = settings.Value;
        }

        public string GenerateToken(LoginResponse response)
        {
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey)), SecurityAlgorithms.HmacSha256);


            var claims = new[]
{
            new Claim(ClaimTypes.NameIdentifier, response.Id.ToString()),

            new Claim("ProfileId", response.ProfileId.ToString()),

            new Claim(ClaimTypes.Email, response.Email),

            new Claim(ClaimTypes.Role, response.Role),

            new Claim("UserName", response.UserName)           
};

            var securityToken = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryTime),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
