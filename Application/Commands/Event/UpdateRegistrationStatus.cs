using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Event
{
    public class UpdateRegistrationStatus
    {
        public record UpdateRegistrationStatusCommand(
            Guid UserId,
            Guid RegistrationId,
            EventRegistrationStatus Status
        ) : IRequest<Result<string>>;

        public class UpdateRegistrationStatusHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRegistrationRepository eventRegistrationRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateRegistrationStatusCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                UpdateRegistrationStatusCommand request,
                CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.UserId);

                if (recruiterProfile is null || recruiterProfile.CompanyId is null)
                    return Result<string>.Failure("Recruiter profile not found");

                var registration = await eventRegistrationRepository.GetByIdAsync(request.RegistrationId);

                if (registration is null)
                    return Result<string>.Failure("Registration not found");

                if (registration.Event.CompanyId != recruiterProfile.CompanyId)
                    return Result<string>.Failure("You are not authorized to manage this registration");

                if (request.Status != EventRegistrationStatus.Attended && request.Status != EventRegistrationStatus.NoShow)
                    return Result<string>.Failure("Status can only be set to Attended or NoShow");

                registration.Status = request.Status;

                eventRegistrationRepository.Update(registration);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("updated", "Registration status updated");
            }
        }
    }
}