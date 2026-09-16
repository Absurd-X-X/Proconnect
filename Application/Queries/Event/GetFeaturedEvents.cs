using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Event
{
    public class GetFeaturedEvents
    {
        public record GetFeaturedEventsQuery(int Count = 3) : IRequest<Result<List<DiscoverEventItemResponse>>>;

        public class GetFeaturedEventsHandler(IEventRepository eventRepository)
            : IRequestHandler<GetFeaturedEventsQuery, Result<List<DiscoverEventItemResponse>>>
        {
            public async Task<Result<List<DiscoverEventItemResponse>>> Handle(
                GetFeaturedEventsQuery request,
                CancellationToken cancellationToken)
            {
                var page = await eventRepository.SearchAsync(
                new PageRequest { PageNumber = 1, PageSize = request.Count },
                usePaging: true,
                searchTerm: null,
                eventTypes: null,
                locationType: null,
                location: null,
                startDateFrom: DateTime.UtcNow,
                startDateTo: null);

                var items = page.Items.Select(e => new DiscoverEventItemResponse(
                    e.Id, e.Title, e.Description, e.CoverImageUrl, e.EventType, e.Category,
                    e.StartDateTime, e.EndDateTime, e.LocationType, e.Location, e.Company.Name,
                    e.Registrations.Count(r => r.Status == EventRegistrationStatus.Registered), false))
                    .ToList();

                return Result<List<DiscoverEventItemResponse>>.Success(items, "Featured events retrieved successfully");
            }
        }
    }
}