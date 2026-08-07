using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IProfessionalProfileRepository
    {
        Task AddProfessionalProfile(ProfessionalProfile profile);

        Task<ProfessionalProfile?> GetByUserIdAsync(Guid userId);

        void Update(ProfessionalProfile profile);
    }
}