using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IProfessionalSkillRepository
    {
        Task AddAsync(ProfessionalSkill professionalSkill);

        Task<bool> ExistsAsync(
            Guid professionalProfileId,
            Guid skillId);
    }
}