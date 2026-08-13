using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IProfessionalSkillRepository
    {
        Task CreateAsync(ProfessionalSkill skill);

        Task<ProfessionalSkill?> GetByIdAsync(Guid id);

        Task<PageResponse<ProfessionalSkill>> GetByProfessionalProfileIdAsync(PageRequest request, bool usePaging, Guid professionalProfileId);

        Task<bool> ExistsAsync(Guid professionalProfileId, Guid skillId);

        void Delete(ProfessionalSkill skill);
    }
}