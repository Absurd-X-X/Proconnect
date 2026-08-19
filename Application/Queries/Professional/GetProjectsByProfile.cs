using Application.Common.Pagenation;
using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Queries.Professional
{
    public class GetProjectsByProfile
    {
        public record GetProjectsByProfileQuery(
            Guid ProfessionalProfileId,
            PageRequest PageRequest,
            bool UsePaging
            ) : IRequest<Result<PageResponse<GetProjectsByProfileResponse>>>;

        public class GetProjectsByProfileHandler(
            IProjectRepository projectRepository) : IRequestHandler<GetProjectsByProfileQuery, Result<PageResponse<GetProjectsByProfileResponse>>>
        {
            public async Task<Result<PageResponse<GetProjectsByProfileResponse>>> Handle(GetProjectsByProfileQuery request, CancellationToken cancellationToken)
            {
                var projects = await projectRepository.GetByProfessionalProfileIdAsync(
                    request.ProfessionalProfileId,
                    request.PageRequest,
                    request.UsePaging);

                var items = projects.Items.Select(p => new GetProjectsByProfileResponse(
                    p.Id,
                    p.Title,
                    p.ProjectUrl)).ToList();

                var response = new PageResponse<GetProjectsByProfileResponse>
                {
                    Items = items,
                    TotalCount = projects.TotalCount,
                    PageNumber = projects.PageNumber,
                    PageSize = projects.PageSize
                };

                return Result<PageResponse<GetProjectsByProfileResponse>>.Success(response, "Projects retrieved successfully");
            }
        }
    }

    public record GetProjectsByProfileResponse(
        Guid Id,
        string Title,
        string ProjectUrl);
}