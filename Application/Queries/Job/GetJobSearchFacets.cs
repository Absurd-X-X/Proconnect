using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Job
{
    public class GetJobSearchFacets
    {
        public record GetJobSearchFacetsQuery(
            string? Keyword,
            Guid? JobCategoryId,
            Guid? CompanyId,
            string? Location,
            EmploymentType? EmploymentType,
            ExperienceLevel? ExperienceLevel,
            decimal? MinSalary,
            decimal? MaxSalary,
            DateTime? PostedAfter
        ) : IRequest<Result<GetJobSearchFacetsResponse>>;

        public class GetJobSearchFacetsHandler(
            IJobRepository jobRepository)
            : IRequestHandler<GetJobSearchFacetsQuery, Result<GetJobSearchFacetsResponse>>
        {
            public async Task<Result<GetJobSearchFacetsResponse>> Handle(
                GetJobSearchFacetsQuery request,
                CancellationToken cancellationToken)
            {
                var filter = new JobSearchFilter
                {
                    Keyword = request.Keyword,
                    JobCategoryId = request.JobCategoryId,
                    CompanyId = request.CompanyId,
                    Location = request.Location,
                    EmploymentType = request.EmploymentType,
                    ExperienceLevel = request.ExperienceLevel,
                    MinSalary = request.MinSalary,
                    MaxSalary = request.MaxSalary,
                    PostedAfter = request.PostedAfter
                };

                var counts = await jobRepository.GetWorkPlaceTypeCountsAsync(filter);

                int Count(WorkPlaceType type) => counts.TryGetValue(type, out var c) ? c : 0;

                var response = new GetJobSearchFacetsResponse(
                    Count(WorkPlaceType.Remote),
                    Count(WorkPlaceType.Onsite),
                    Count(WorkPlaceType.Hybrid));

                return Result<GetJobSearchFacetsResponse>.Success(response, "Facets retrieved successfully");
            }
        }
    }

    public record GetJobSearchFacetsResponse(int Remote, int Onsite, int Hybrid);
}