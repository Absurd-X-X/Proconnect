using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Social
{
    public class GetComments
    {
        public record GetCommentsQuery(Guid PostId, PageRequest PageRequest, bool UsePaging)
            : IRequest<Result<PageResponse<CommentItemResponse>>>;

        public class GetCommentsHandler(
            ICommentRepository commentRepository) : IRequestHandler<GetCommentsQuery, Result<PageResponse<CommentItemResponse>>>
        {
            public async Task<Result<PageResponse<CommentItemResponse>>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
            {
                var page = await commentRepository.GetByPostIdAsync(request.PostId, request.PageRequest, request.UsePaging);

                var items = page.Items.Select(c => new CommentItemResponse(
                    c.Id,
                    c.User.Id,
                    c.User.FirstName,
                    c.User.LastName,
                    c.User.ProfilePictureUrl,
                    c.Content,
                    c.DateCreated,
                    c.DateUpdated)).ToList();

                var response = new PageResponse<CommentItemResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<CommentItemResponse>>.Success(response, "Comments retrieved successfully");
            }
        }
    }

    public record CommentItemResponse(
        Guid Id,
        Guid AuthorId,
        string AuthorFirstName,
        string AuthorLastName,
        string? AuthorProfilePictureUrl,
        string Content,
        DateTime DateCreated,
        DateTime DateUpdated);
}