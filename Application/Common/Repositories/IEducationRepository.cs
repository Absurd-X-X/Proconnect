using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IEducationRepository
    {
        Task AddAsync(Education education);

        Task<Education?> GetByIdAsync(Guid id);

        void Update(Education education);
    }
}