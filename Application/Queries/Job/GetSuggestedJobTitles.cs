using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Job
{
    public class GetSuggestedJobTitles
    {
        public record GetSuggestedJobTitlesQuery(
            Guid? ProfessionalProfileId
        ) : IRequest<Result<List<GetSuggestedJobTitlesResponse>>>;

        public class GetSuggestedJobTitlesHandler(
            IJobRepository jobRepository,
            ISavedJobRepository savedJobRepository,
            IJobApplicationRepository jobApplicationRepository)
            : IRequestHandler<GetSuggestedJobTitlesQuery, Result<List<GetSuggestedJobTitlesResponse>>>
        {
            public async Task<Result<List<GetSuggestedJobTitlesResponse>>> Handle(
                GetSuggestedJobTitlesQuery request,
                CancellationToken cancellationToken)
            {
                List<Guid>? categoryIds = null;

                if (request.ProfessionalProfileId.HasValue)
                {
                    var pageRequest = new Common.Pagenation.PageRequest { PageNumber = 1, PageSize = 1 };

                    var saved = await savedJobRepository.GetByProfessionalProfileIdAsync(
                        request.ProfessionalProfileId.Value, pageRequest, usePaging: false);

                    var applied = await jobApplicationRepository.GetByProfessionalProfileIdAsync(
                        request.ProfessionalProfileId.Value, pageRequest, usePaging: false);

                    var ids = saved.Items.Select(s => s.Job.JobCategoryId)
                        .Concat(applied.Items.Select(a => a.Job.JobCategoryId))
                        .Distinct()
                        .ToList();

                    if (ids.Count > 0)
                        categoryIds = ids;
                }

                var trending = await jobRepository.GetTrendingJobTitlesAsync(categoryIds, take: 5);

                if (trending.Count < 3 && categoryIds is not null)
                    trending = await jobRepository.GetTrendingJobTitlesAsync(null, take: 5);

                var response = trending
                    .Select(t => new GetSuggestedJobTitlesResponse(t.Title, t.Count))
                    .ToList();

                return Result<List<GetSuggestedJobTitlesResponse>>.Success(response, "Suggestions retrieved successfully");
            }
        }
    }

    public record GetSuggestedJobTitlesResponse(string Title, int JobCount);
}