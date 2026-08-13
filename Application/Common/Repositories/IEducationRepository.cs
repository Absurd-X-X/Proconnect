using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IEducationRepository
    {
        Task CreateAsync(Education education);

        Task<Education?> GetByIdAsync(Guid id);

        Task<PageResponse<Education>> GetByProfessionalProfileIdAsync(PageRequest request, bool usePaging, Guid professionalProfileId);

        void UpdateAsync(Education education);

        void Delete(Education education);
    }
}