using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Job
{
    public class DeleteJobAlert
    {
        public record DeleteJobAlertCommand(Guid AlertId, Guid ProfessionalProfileId) : IRequest<Result<string>>;

        public class DeleteJobAlertHandler(
            ISavedJobSearchRepository savedJobSearchRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<DeleteJobAlertCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(DeleteJobAlertCommand request, CancellationToken cancellationToken)
            {
                var search = await savedJobSearchRepository.GetByIdAsync(request.AlertId);

                if (search is null)
                    return Result<string>.Failure("Job alert not found");

                if (search.ProfessionalProfileId != request.ProfessionalProfileId)
                    return Result<string>.Failure("You are not authorized to delete this alert");

                savedJobSearchRepository.Delete(search);
                await unitOfWork.SaveAsync();

                return Result<string>.Success("Deleted", "Job alert deleted successfully");
            }
        }
    }
}