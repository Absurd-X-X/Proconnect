using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface ISavedJobSearchRepository
    {
        Task AddAsync(SavedJobSearch search);
        Task<SavedJobSearch?> GetByIdAsync(Guid id);
        Task<List<SavedJobSearch>> GetByProfessionalProfileIdAsync(Guid professionalProfileId);
        Task<List<SavedJobSearch>> GetAllActiveAsync();
        void Delete(SavedJobSearch search);
        void Update(SavedJobSearch search);
    }
}