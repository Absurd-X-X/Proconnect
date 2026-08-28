using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Networking
{
    public class GetMyFollowing
    {
        public record GetMyFollowingQuery(Guid UserId, PageRequest PageRequest, bool UsePaging) : IRequest<Result<PageResponse<FollowingResponse>>>;

        public class GetMyFollowingHandler(
            IUserFollowRepository followRepository) : IRequestHandler<GetMyFollowingQuery, Result<PageResponse<FollowingResponse>>>
        {
            public async Task<Result<PageResponse<FollowingResponse>>> Handle(GetMyFollowingQuery request, CancellationToken cancellationToken)
            {
                var page = await followRepository.GetFollowingAsync(request.UserId, request.PageRequest, request.UsePaging);

                var items = page.Items.Select(f => new FollowingResponse(
                    f.Id,
                    f.Following.Id,
                    f.Following.FirstName,
                    f.Following.LastName,
                    f.Following.ProfilePictureUrl,
                    f.DateCreated)).ToList();

                var response = new PageResponse<FollowingResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<FollowingResponse>>.Success(response, "Following list retrieved successfully");
            }
        }
    }

    public record FollowingResponse(
        Guid FollowId,
        Guid UserId,
        string FirstName,
        string LastName,
        string? ProfilePictureUrl,
        DateTime DateCreated);
}