using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface ISavedEventRepository
    {
        Task AddAsync(SavedEvent savedEvent);

        Task<bool> ExistsAsync(Guid eventId, Guid professionalProfileId);

        Task<SavedEvent?> GetByEventAndProfileAsync(Guid eventId, Guid professionalProfileId);

        Task<PageResponse<SavedEvent>> GetByProfessionalProfileIdAsync(
            PageRequest request,
            bool usePaging,
            Guid professionalProfileId);

        void Delete(SavedEvent savedEvent);
    }
}