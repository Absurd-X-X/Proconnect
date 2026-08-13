using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IExperienceRepository
    {
        Task CreateAsync(Experience experience);

        Task<Experience?> GetByIdAsync(Guid id);

        Task<PageResponse<Experience>> GetByProfessionalProfileIdAsync(PageRequest request, bool usePaging, Guid professionalProfileId);

        void UpdateAsync(Experience experience);

        void Delete(Experience experience);
    }
}