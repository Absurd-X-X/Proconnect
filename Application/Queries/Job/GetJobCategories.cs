using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Job
{
    public class GetJobCategories
    {
        public record GetJobCategoriesQuery(
            EmploymentType? EmploymentType,
            ExperienceLevel? ExperienceLevel
        ) : IRequest<Result<List<GetJobCategoriesResponse>>>;

        public class GetJobCategoriesHandler(
            IJobCategoryRepository jobCategoryRepository)
            : IRequestHandler<GetJobCategoriesQuery, Result<List<GetJobCategoriesResponse>>>
        {
            public async Task<Result<List<GetJobCategoriesResponse>>> Handle(
                GetJobCategoriesQuery request,
                CancellationToken cancellationToken)
            {
                var categories = await jobCategoryRepository.GetAllAsync();

                var counts = await jobCategoryRepository.GetActiveJobCountsAsync(
                    request.EmploymentType, request.ExperienceLevel);

                var isFiltered = request.EmploymentType.HasValue || request.ExperienceLevel.HasValue;

                var response = categories
                    .Select(c => new GetJobCategoriesResponse(
                        c.Id,
                        c.Name,
                        c.Description,
                        counts.TryGetValue(c.Id, out var count) ? count : 0))
                    .Where(c => !isFiltered || c.JobCount > 0)
                    .OrderByDescending(c => c.JobCount)
                    .ToList();

                return Result<List<GetJobCategoriesResponse>>.Success(response, "Job categories retrieved successfully");
            }
        }
    }

    public record GetJobCategoriesResponse(Guid Id, string Name, string Description, int JobCount);
}