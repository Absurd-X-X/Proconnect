using Application.Common.Dtos;

namespace Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task<Result<string>> SendEmailAsync(EmailRequest request);
    }
}