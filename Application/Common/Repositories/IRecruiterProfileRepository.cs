using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IRecruiterProfileRepository
    {
        Task AddAsync(RecruiterProfile recruiterProfile);

        Task<RecruiterProfile?> GetByUserIdAsync(Guid userId);
    }
}