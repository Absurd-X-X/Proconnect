using Application.Common.Pagenation;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Repositories
{
    public interface IEventRegistrationRepository
    {
        Task AddAsync(EventRegistration registration);

        Task<EventRegistration?> GetByIdAsync(Guid id);

        Task<bool> ExistsAsync(Guid eventId, Guid professionalProfileId);

        Task<EventRegistration?> GetByEventAndProfileAsync(Guid eventId, Guid professionalProfileId);

        Task<PageResponse<EventRegistration>> GetByEventIdAsync(
            PageRequest request,
            bool usePaging,
            Guid eventId,
            EventRegistrationStatus? status);

        Task<PageResponse<EventRegistration>> GetByProfessionalProfileIdAsync(
            PageRequest request,
            bool usePaging,
            Guid professionalProfileId,
            EventRegistrationStatus? status,
            DateTime? eventStartDateFrom,
            DateTime? eventStartDateTo);

        void Update(EventRegistration registration);
    }
}