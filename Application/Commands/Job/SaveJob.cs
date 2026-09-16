using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Job
{
    public class SaveJob
    {
        public record SaveJobCommand(
            Guid JobId,
            Guid ProfessionalProfileId,
            string CreatedBy
        ) : IRequest<Result<string>>;

        public class SaveJobHandler(
            IJobRepository jobRepository,
            ISavedJobRepository savedJobRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<SaveJobCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                SaveJobCommand request,
                CancellationToken cancellationToken)
            {
                var job = await jobRepository.GetByIdAsync(request.JobId);

                if (job is null)
                    return Result<string>.Failure("Job not found");

                var existing = await savedJobRepository.GetByProfessionalAndJobAsync(
                    request.ProfessionalProfileId, request.JobId);

                if (existing is not null)
                    return Result<string>.Failure("Job already saved");

                var savedJob = new SavedJob
                {
                    ProfessionalProfileId = request.ProfessionalProfileId,

                    JobId = request.JobId,

                    CreatedBy = request.CreatedBy,

                    SavedAt = DateTime.UtcNow
                };

                await savedJobRepository.AddAsync(savedJob);

                await unitOfWork.SaveAsync();

                return Result<string>.Success(savedJob.Id.ToString(), "Job saved successfully");
            }
        }
    }
}