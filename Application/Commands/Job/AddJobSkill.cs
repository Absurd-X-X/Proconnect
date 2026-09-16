using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Job
{
    public class AddJobSkill
    {
        public record AddJobSkillCommand(
            Guid JobId,
            Guid RecruiterProfileId,
            Guid SkillId,
            string CreatedBy
        ) : IRequest<Result<string>>;

        public class AddJobSkillHandler(
            IJobRepository jobRepository,
            IJobSkillRepository jobSkillRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<AddJobSkillCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                AddJobSkillCommand request,
                CancellationToken cancellationToken)
            {
                var job = await jobRepository.GetByIdAsync(request.JobId);

                if (job is null)
                    return Result<string>.Failure("Job not found");

                if (job.RecruiterProfileId != request.RecruiterProfileId)
                    return Result<string>.Failure("You are not authorized to edit this job");

                var exists = await jobSkillRepository.ExistsAsync(request.JobId, request.SkillId);

                if (exists)
                    return Result<string>.Failure("Skill already added to this job");

                var jobSkill = new JobSkill
                {
                    JobId = request.JobId,

                    SkillId = request.SkillId,

                    CreatedBy = request.CreatedBy,

                    DateCreated = DateTime.UtcNow
                };

                await jobSkillRepository.AddAsync(jobSkill);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Skill added successfully", "created");
            }
        }
    }
}