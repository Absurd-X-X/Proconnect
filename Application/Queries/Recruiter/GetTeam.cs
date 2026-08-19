using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Queries
{
    public class GetTeam
    {
        public record TeamMemberResponse(
            Guid Id,
            Guid UserId,
            string FullName,
            string Email,
            string? ProfilePictureUrl,
            string? JobTitle,
            string? Department,
            bool IsCompanyAdmin,
            RecruiterStatus Status,
            DateTime DateCreated
        );

        public record GetTeamQuery(
            Guid RequestingUserId,
            RecruiterStatus? Status,
            int PageNumber,
            int PageSize,
            bool UsePaging
        ) : IRequest<Result<PageResponse<TeamMemberResponse>>>;

        public class GetTeamHandler(
            IRecruiterProfileRepository recruiterProfileRepository)
            : IRequestHandler<GetTeamQuery, Result<PageResponse<TeamMemberResponse>>>
        {
            public async Task<Result<PageResponse<TeamMemberResponse>>> Handle(
                GetTeamQuery request,
                CancellationToken cancellationToken)
            {
                var requestingProfile = await recruiterProfileRepository
                    .GetByUserIdAsync(request.RequestingUserId);

                if (requestingProfile is null || requestingProfile.CompanyId is null)
                {
                    return Result<PageResponse<TeamMemberResponse>>.Failure("You are not linked to a company");
                }

                var pageRequest = new PageRequest
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                var page = await recruiterProfileRepository.GetByCompanyIdAsync(
                    pageRequest,
                    request.UsePaging,
                    requestingProfile.CompanyId.Value,
                    request.Status);

                var items = page.Items.Select(r => new TeamMemberResponse(
                    r.Id,
                    r.UserId,
                    $"{r.User.FirstName} {r.User.LastName}",
                    r.User.Email,
                    r.User.ProfilePictureUrl,
                    r.JobTitle,
                    r.Department,
                    r.IsCompanyAdmin,
                    r.Status,
                    r.DateCreated)).ToList();

                var result = new PageResponse<TeamMemberResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<TeamMemberResponse>>.Success(result, "Team retrieved");
            }
        }
    }
}