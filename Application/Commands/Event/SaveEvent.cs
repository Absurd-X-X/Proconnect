using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Event
{
    public class SaveEvent
    {
        public record SaveEventCommand(Guid UserId, Guid EventId) : IRequest<Result<string>>;

        public class SaveEventHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IEventRepository eventRepository,
            ISavedEventRepository savedEventRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<SaveEventCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(SaveEventCommand request, CancellationToken cancellationToken)
            {
                var professionalProfile = await professionalProfileRepository.GetByUserIdAsync(request.UserId);

                if (professionalProfile is null)
                    return Result<string>.Failure("Professional profile not found");

                var @event = await eventRepository.GetByIdAsync(request.EventId);

                if (@event is null)
                    return Result<string>.Failure("Event not found");

                var alreadySaved = await savedEventRepository.ExistsAsync(request.EventId, professionalProfile.Id);

                if (alreadySaved)
                    return Result<string>.Failure("Event already saved");

                var savedEvent = new Domain.Entities.SavedEvent
                {
                    EventId = request.EventId,
                    ProfessionalProfileId = professionalProfile.Id,
                    CreatedBy = request.UserId.ToString()
                };

                await savedEventRepository.AddAsync(savedEvent);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("saved", "Event saved");
            }
        }
    }
}