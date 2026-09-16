using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Event
{
    public class DiscoverEvents
    {
        public record DiscoverEventsQuery(
        Guid UserId,
        PageRequest PageRequest,
        bool UsePaging,
        string? SearchTerm,
        List<EventType>? EventTypes,
        EventLocationType? LocationType,
        string? Location,
        DateTime? StartDateFrom,
        DateTime? StartDateTo
    ) : IRequest<Result<PageResponse<DiscoverEventItemResponse>>>;

        public class DiscoverEventsHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IEventRepository eventRepository,
            ISavedEventRepository savedEventRepository)
            : IRequestHandler<DiscoverEventsQuery, Result<PageResponse<DiscoverEventItemResponse>>>
        {
            public async Task<Result<PageResponse<DiscoverEventItemResponse>>> Handle(
                DiscoverEventsQuery request,
                CancellationToken cancellationToken)
            {
                var professionalProfile = await professionalProfileRepository.GetByUserIdAsync(request.UserId);

                var page = await eventRepository.SearchAsync(
                request.PageRequest,
                request.UsePaging,
                request.SearchTerm,
                request.EventTypes,
                request.LocationType,
                request.Location,
                request.StartDateFrom,
                request.StartDateTo);

                var items = new List<DiscoverEventItemResponse>();

                foreach (var e in page.Items)
                {
                    var isSaved = professionalProfile is not null
                        && await savedEventRepository.ExistsAsync(e.Id, professionalProfile.Id);

                    items.Add(new DiscoverEventItemResponse(
                        e.Id,
                        e.Title,
                        e.Description,
                        e.CoverImageUrl,
                        e.EventType,
                        e.Category,
                        e.StartDateTime,
                        e.EndDateTime,
                        e.LocationType,
                        e.Location,
                        e.Company.Name,
                        e.Registrations.Count(r => r.Status == EventRegistrationStatus.Registered),
                        isSaved));
                }

                var response = new PageResponse<DiscoverEventItemResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<DiscoverEventItemResponse>>.Success(response, "Events retrieved successfully");
            }
        }
    }

    public record DiscoverEventItemResponse(
        Guid Id,
        string Title,
        string Description,
        string? CoverImageUrl,
        EventType EventType,
        string? Category,
        DateTime StartDateTime,
        DateTime EndDateTime,
        EventLocationType LocationType,
        string? Location,
        string CompanyName,
        int GoingCount,
        bool IsSaved);
}