using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Job
{
    public class ToggleJobAlert
    {
        public record ToggleJobAlertCommand(Guid AlertId, Guid ProfessionalProfileId, bool Enabled) : IRequest<Result<string>>;

        public class ToggleJobAlertHandler(
            ISavedJobSearchRepository savedJobSearchRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<ToggleJobAlertCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(ToggleJobAlertCommand request, CancellationToken cancellationToken)
            {
                var search = await savedJobSearchRepository.GetByIdAsync(request.AlertId);

                if (search is null)
                    return Result<string>.Failure("Job alert not found");

                if (search.ProfessionalProfileId != request.ProfessionalProfileId)
                    return Result<string>.Failure("You are not authorized to update this alert");

                search.EmailNotificationsEnabled = request.Enabled;
                savedJobSearchRepository.Update(search);
                await unitOfWork.SaveAsync();

                return Result<string>.Success("Updated", "Job alert updated successfully");
            }
        }
    }
}