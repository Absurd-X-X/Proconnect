using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface ICertificateRepository
    {
        Task AddAsync(Certificate certificate);
    }
}