using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Networking
{
    public class GetMyFollowers
    {
        public record GetMyFollowersQuery(Guid UserId, PageRequest PageRequest, bool UsePaging) : IRequest<Result<PageResponse<FollowerResponse>>>;

        public class GetMyFollowersHandler(
            IUserFollowRepository followRepository) : IRequestHandler<GetMyFollowersQuery, Result<PageResponse<FollowerResponse>>>
        {
            public async Task<Result<PageResponse<FollowerResponse>>> Handle(GetMyFollowersQuery request, CancellationToken cancellationToken)
            {
                var page = await followRepository.GetFollowersAsync(request.UserId, request.PageRequest, request.UsePaging);

                var items = page.Items.Select(f => new FollowerResponse(
                    f.Id,
                    f.Follower.Id,
                    f.Follower.FirstName,
                    f.Follower.LastName,
                    f.Follower.ProfilePictureUrl,
                    f.DateCreated)).ToList();

                var response = new PageResponse<FollowerResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<FollowerResponse>>.Success(response, "Followers retrieved successfully");
            }
        }
    }

    public record FollowerResponse(
        Guid FollowId,
        Guid UserId,
        string FirstName,
        string LastName,
        string? ProfilePictureUrl,
        DateTime DateCreated);
}