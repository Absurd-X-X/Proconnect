using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface ICertificateRepository
    {
        Task CreateAsync(Certificate certificate);

        Task<Certificate?> GetByIdAsync(Guid id);

        Task<PageResponse<Certificate>> GetByProfessionalProfileIdAsync(PageRequest request, bool usePaging, Guid professionalProfileId);

        void UpdateAsync(Certificate certificate);

        void Delete(Certificate certificate);
    }
}