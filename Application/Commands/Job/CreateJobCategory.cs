using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Job
{
    public class CreateJobCategory
    {
        public record CreateJobCategoryCommand(
            string Name,
            string Description,
            string CreatedBy
        ) : IRequest<Result<string>>;

        public class CreateJobCategoryHandler(
            IJobCategoryRepository jobCategoryRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CreateJobCategoryCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                CreateJobCategoryCommand request,
                CancellationToken cancellationToken)
            {
                var exists = await jobCategoryRepository.ExistsByNameAsync(request.Name);

                if (exists)
                    return Result<string>.Failure("A job category with this name already exists");

                var category = new Domain.Entities.JobCategory
                {
                    Name = request.Name,

                    Description = request.Description,

                    CreatedBy = request.CreatedBy,

                    DateCreated = DateTime.UtcNow
                };

                await jobCategoryRepository.AddAsync(category);

                await unitOfWork.SaveAsync();

                return Result<string>.Success(category.Id.ToString(), "Job category created successfully");
            }
        }
    }
}