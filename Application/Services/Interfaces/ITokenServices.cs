using Application.Contract.Settings;

namespace Application.Services.Interfaces
{
    public interface ITokenServices
    {
        string GenerateToken(LoginResponse response);
    }
}
