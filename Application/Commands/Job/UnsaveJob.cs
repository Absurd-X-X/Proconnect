using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Job
{
    public class UnsaveJob
    {
        public record UnsaveJobCommand(
            Guid JobId,
            Guid ProfessionalProfileId
        ) : IRequest<Result<string>>;

        public class UnsaveJobHandler(
            ISavedJobRepository savedJobRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UnsaveJobCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                UnsaveJobCommand request,
                CancellationToken cancellationToken)
            {
                var savedJob = await savedJobRepository.GetByProfessionalAndJobAsync(
                    request.ProfessionalProfileId, request.JobId);

                if (savedJob is null)
                    return Result<string>.Failure("Saved job not found");

                savedJobRepository.Delete(savedJob);

                await unitOfWork.SaveAsync();

                return Result<string>.Success(savedJob.Id.ToString(), "Job removed from saved list");
            }
        }
    }
}