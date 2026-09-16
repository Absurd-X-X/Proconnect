using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Event
{
    public class GetMyEvents
    {
        public enum MyEventsTab
        {
            Registered,
            Saved,
            Upcoming,
            Past
        }

        public record GetMyEventsQuery(
            Guid UserId,
            PageRequest PageRequest,
            bool UsePaging,
            MyEventsTab Tab
        ) : IRequest<Result<PageResponse<MyEventItemResponse>>>;

        public class GetMyEventsHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IEventRegistrationRepository eventRegistrationRepository,
            ISavedEventRepository savedEventRepository)
            : IRequestHandler<GetMyEventsQuery, Result<PageResponse<MyEventItemResponse>>>
        {
            public async Task<Result<PageResponse<MyEventItemResponse>>> Handle(
                GetMyEventsQuery request,
                CancellationToken cancellationToken)
            {
                var professionalProfile = await professionalProfileRepository.GetByUserIdAsync(request.UserId);

                if (professionalProfile is null)
                    return Result<PageResponse<MyEventItemResponse>>.Failure("Professional profile not found");

                if (request.Tab == MyEventsTab.Saved)
                {
                    var savedPage = await savedEventRepository.GetByProfessionalProfileIdAsync(
                        request.PageRequest, request.UsePaging, professionalProfile.Id);

                    var savedItems = savedPage.Items.Select(s => new MyEventItemResponse(
                        s.Event.Id,
                        s.Event.Title,
                        s.Event.CoverImageUrl,
                        s.Event.EventType,
                        s.Event.StartDateTime,
                        s.Event.EndDateTime,
                        s.Event.LocationType,
                        s.Event.Location,
                        s.Event.Company.Name,
                        null))
                        .ToList();

                    return Result<PageResponse<MyEventItemResponse>>.Success(
                        new PageResponse<MyEventItemResponse>
                        {
                            Items = savedItems,
                            TotalCount = savedPage.TotalCount,
                            PageNumber = savedPage.PageNumber,
                            PageSize = savedPage.PageSize
                        },
                        "Saved events retrieved successfully");
                }

                var now = DateTime.UtcNow;

                var (startDateFrom, startDateTo) = request.Tab switch
                {
                    MyEventsTab.Upcoming => ((DateTime?)now, (DateTime?)null),
                    MyEventsTab.Past => (null, (DateTime?)now),
                    _ => (null, null)
                };

                var registrationsPage = await eventRegistrationRepository.GetByProfessionalProfileIdAsync(
                    request.PageRequest,
                    request.UsePaging,
                    professionalProfile.Id,
                    EventRegistrationStatus.Registered,
                    startDateFrom,
                    startDateTo);

                var items = registrationsPage.Items.Select(r => new MyEventItemResponse(
                    r.Event.Id,
                    r.Event.Title,
                    r.Event.CoverImageUrl,
                    r.Event.EventType,
                    r.Event.StartDateTime,
                    r.Event.EndDateTime,
                    r.Event.LocationType,
                    r.Event.Location,
                    r.Event.Company.Name,
                    r.Status))
                    .ToList();

                var response = new PageResponse<MyEventItemResponse>
                {
                    Items = items,
                    TotalCount = registrationsPage.TotalCount,
                    PageNumber = registrationsPage.PageNumber,
                    PageSize = registrationsPage.PageSize
                };

                return Result<PageResponse<MyEventItemResponse>>.Success(response, "Events retrieved successfully");
            }
        }
    }

    public record MyEventItemResponse(
        Guid Id,
        string Title,
        string? CoverImageUrl,
        EventType EventType,
        DateTime StartDateTime,
        DateTime EndDateTime,
        EventLocationType LocationType,
        string? Location,
        string CompanyName,
        EventRegistrationStatus? RegistrationStatus);
}