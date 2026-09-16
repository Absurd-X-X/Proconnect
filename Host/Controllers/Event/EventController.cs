using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Queries.Analytics;
using Application.Queries.Event;
using Host.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Commands.Event.AddEventAgendaItem;
using static Application.Commands.Event.AddEventSpeaker;
using static Application.Commands.Event.CancelEvent;
using static Application.Commands.Event.CancelEventRegistration;
using static Application.Commands.Event.CreateEvent;
using static Application.Commands.Event.DeleteEvent;
using static Application.Commands.Event.PublishEvent;
using static Application.Commands.Event.RegisterForEvent;
using static Application.Commands.Event.RemoveEventAgendaItem;
using static Application.Commands.Event.RemoveEventSpeaker;
using static Application.Commands.Event.SaveEvent;
using static Application.Commands.Event.UnsaveEvent;
using static Application.Commands.Event.UpdateEvent;
using static Application.Commands.Event.UpdateEventAgendaItem;
using static Application.Commands.Event.UpdateEventSpeaker;
using static Application.Commands.Event.UpdateRegistrationStatus;
using static Application.Commands.Event.UploadEventCoverImage;
using static Application.Commands.Event.UploadEventRegistrationResume;
using static Application.Queries.Event.DiscoverEvents;
using static Application.Queries.Event.GetEventAttendees;
using static Application.Queries.Event.GetEventDetails;
using static Application.Queries.Event.GetEventForManage;
using static Application.Queries.Event.GetEventRegistrations;
using static Application.Queries.Event.GetFeaturedEvents;
using static Application.Queries.Event.GetManagedEvents;
using static Application.Queries.Event.GetMyEvents;

namespace Host.Controllers.Event
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EventController(IMediator mediator) : ControllerBase
    {
        [HttpPost("create-event")]
        public async Task<IActionResult> CreateEvent(CreateEventCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("update-event")]
        public async Task<IActionResult> UpdateEvent(UpdateEventCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("publish-event")]
        public async Task<IActionResult> PublishEvent(PublishEventCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("cancel-event")]
        public async Task<IActionResult> CancelEvent(CancelEventCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("delete-event")]
        public async Task<IActionResult> DeleteEvent(DeleteEventCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("upload-cover-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCoverImage([FromForm] UploadEventCoverImageCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("managed-events")]
        public async Task<IActionResult> GetManagedEvents(
    [FromQuery] Domain.Enums.EventStatus? status,
    [FromQuery] Domain.Enums.EventType? eventType,
    [FromQuery] string? searchTerm,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetManagedEventsQuery(ClaimsHelper.GetUserId(User), pageRequest, usePaging, status, eventType, searchTerm));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("manage/{id}")]
        public async Task<IActionResult> GetEventForManage(Guid id)
        {
            var response = await mediator.Send(new GetEventForManageQuery(ClaimsHelper.GetUserId(User), id));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("add-speaker")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddSpeaker([FromForm] AddEventSpeakerCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("update-speaker")]
        public async Task<IActionResult> UpdateSpeaker(UpdateEventSpeakerCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("remove-speaker")]
        public async Task<IActionResult> RemoveSpeaker(RemoveEventSpeakerCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("add-agenda-item")]
        public async Task<IActionResult> AddAgendaItem(AddEventAgendaItemCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("update-agenda-item")]
        public async Task<IActionResult> UpdateAgendaItem(UpdateEventAgendaItemCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("remove-agenda-item")]
        public async Task<IActionResult> RemoveAgendaItem(RemoveEventAgendaItemCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("update-registration-status")]
        public async Task<IActionResult> UpdateRegistrationStatus(UpdateRegistrationStatusCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("registrations/{eventId}")]
        public async Task<IActionResult> GetEventRegistrations(
            Guid eventId,
            [FromQuery] Domain.Enums.EventRegistrationStatus? status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };
            var response = await mediator.Send(
                new GetEventRegistrationsQuery(ClaimsHelper.GetUserId(User), eventId, pageRequest, usePaging, status));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("upload-registration-resume")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadRegistrationResume([FromForm] UploadEventRegistrationResumeCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterForEvent(RegisterForEventCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("cancel-registration")]
        public async Task<IActionResult> CancelRegistration(CancelEventRegistrationCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveEvent(SaveEventCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("unsave")]
        public async Task<IActionResult> UnsaveEvent(UnsaveEventCommand command)
        {
            var response = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("discover")]
        public async Task<IActionResult> DiscoverEvents(
    [FromQuery] string? searchTerm,
    [FromQuery] string? eventTypes,
    [FromQuery] Domain.Enums.EventLocationType? locationType,
    [FromQuery] string? location,
    [FromQuery] DateTime? startDateFrom,
    [FromQuery] DateTime? startDateTo,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] bool usePaging = true)
        {
            List<Domain.Enums.EventType>? parsedTypes = null;

            if (!string.IsNullOrWhiteSpace(eventTypes))
            {
                parsedTypes = eventTypes
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => Enum.TryParse<Domain.Enums.EventType>(t.Trim(), true, out var parsed) ? parsed : (Domain.Enums.EventType?)null)
                    .Where(t => t.HasValue)
                    .Select(t => t!.Value)
                    .ToList();
            }

            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };
            var response = await mediator.Send(new DiscoverEventsQuery(
                ClaimsHelper.GetUserId(User), pageRequest, usePaging, searchTerm, parsedTypes, locationType, location, startDateFrom, startDateTo));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedEvents([FromQuery] int count = 3)
        {
            var response = await mediator.Send(new GetFeaturedEventsQuery(count));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventDetails(Guid id)
        {
            var response = await mediator.Send(new GetEventDetailsQuery(ClaimsHelper.GetUserId(User), id));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("my-events")]
        public async Task<IActionResult> GetMyEvents(
            [FromQuery] MyEventsTab tab,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };
            var response = await mediator.Send(new GetMyEventsQuery(ClaimsHelper.GetUserId(User), pageRequest, usePaging, tab));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("attendees/{eventId}")]
        public async Task<IActionResult> GetEventAttendees(
    Guid eventId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 12,
    [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };
            var response = await mediator.Send(new GetEventAttendeesQuery(eventId, pageRequest, usePaging));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("recruiter/job-views")]
        public async Task<IActionResult> GetJobViews(
    [FromQuery] Guid recruiterProfileId,
    [FromQuery] DateRangePreset preset = DateRangePreset.Last30Days,
    [FromQuery] DateTime? customStart = null,
    [FromQuery] DateTime? customEnd = null,
    [FromQuery] Guid? jobId = null)
        {
            var query = new GetJobViewsAnalytics.GetJobViewsAnalyticsQuery(
                recruiterProfileId, preset, customStart, customEnd, jobId);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }
    }
}