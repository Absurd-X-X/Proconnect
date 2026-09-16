using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface ISavedJobRepository
    {
        Task AddAsync(SavedJob savedJob);
        Task<SavedJob?> GetByProfessionalAndJobAsync(Guid professionalProfileId, Guid jobId);
        Task<PageResponse<SavedJob>> GetByProfessionalProfileIdAsync(Guid professionalProfileId, PageRequest request, bool usePaging);
        void Delete(SavedJob savedJob);
    }
}