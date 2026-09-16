using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Event
{
    public class UnsaveEvent
    {
        public record UnsaveEventCommand(Guid UserId, Guid EventId) : IRequest<Result<string>>;

        public class UnsaveEventHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            ISavedEventRepository savedEventRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UnsaveEventCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UnsaveEventCommand request, CancellationToken cancellationToken)
            {
                var professionalProfile = await professionalProfileRepository.GetByUserIdAsync(request.UserId);

                if (professionalProfile is null)
                    return Result<string>.Failure("Professional profile not found");

                var savedEvent = await savedEventRepository.GetByEventAndProfileAsync(request.EventId, professionalProfile.Id);

                if (savedEvent is null)
                    return Result<string>.Failure("Event is not saved");

                savedEventRepository.Delete(savedEvent);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("unsaved", "Event removed from saved");
            }
        }
    }
}