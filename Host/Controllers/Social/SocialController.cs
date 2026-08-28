using Application.Common.Pagenation;
using Domain.Enums;
using Host.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Commands.Posts.AddComment;
using static Application.Commands.Posts.DeleteComment;
using static Application.Commands.Posts.ReactToPost;
using static Application.Commands.Social.SharePost;
using static Application.Commands.Posts.UpdateComment;
using static Application.Commands.Social.CreatePost;
using static Application.Commands.Social.DeletePost;
using static Application.Commands.Social.UpdatePost;
using static Application.Queries.Social.GetComments;
using static Application.Queries.Social.GetFeed;
using static Application.Queries.Social.GetPostById;
using static Application.Queries.Social.GetPostsByUser;
using static Application.Commands.Posts.RemoveReaction;

namespace Host.Controllers.Posts
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController(IMediator mediator) : ControllerBase
    {
        [Authorize]
        [HttpPost("create-post")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request)
        {
            var command = new CreatePostCommand(
                ClaimsHelper.GetUserId(User),
                request.Content,
                request.Visibility,
                request.Attachments);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("update-post")]
        public async Task<IActionResult> UpdatePost(UpdatePostRequest request)
        {
            var command = new UpdatePostCommand(
                ClaimsHelper.GetUserId(User),
                request.PostId,
                request.Content,
                request.Visibility);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("delete-post")]
        public async Task<IActionResult> DeletePost([FromQuery] Guid postId)
        {
            var command = new DeletePostCommand(ClaimsHelper.GetUserId(User), postId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("share-post")]
        public async Task<IActionResult> SharePost([FromQuery] Guid postId, [FromQuery] string? content)
        {
            var command = new SharePostCommand(ClaimsHelper.GetUserId(User), postId, content);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("react")]
        public async Task<IActionResult> ReactToPost([FromQuery] Guid postId, [FromQuery] ReactionType reactionType)
        {
            var command = new ReactToPostCommand(ClaimsHelper.GetUserId(User), postId, reactionType);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("remove-reaction")]
        public async Task<IActionResult> RemoveReaction([FromQuery] Guid postId)
        {
            var command = new RemoveReactionCommand(ClaimsHelper.GetUserId(User), postId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("add-comment")]
        public async Task<IActionResult> AddComment([FromQuery] Guid postId, [FromQuery] string content)
        {
            var command = new AddCommentCommand(ClaimsHelper.GetUserId(User), postId, content);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("update-comment")]
        public async Task<IActionResult> UpdateComment([FromQuery] Guid commentId, [FromQuery] string content)
        {
            var command = new UpdateCommentCommand(ClaimsHelper.GetUserId(User), commentId, content);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("delete-comment")]
        public async Task<IActionResult> DeleteComment([FromQuery] Guid commentId)
        {
            var command = new DeleteCommentCommand(ClaimsHelper.GetUserId(User), commentId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("feed")]
        public async Task<IActionResult> GetFeed(
            [FromQuery] FeedTab tab = FeedTab.ForYou,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetFeedQuery(ClaimsHelper.GetUserId(User), tab, pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetPost(Guid postId)
        {
            var response = await mediator.Send(new GetPostByIdQuery(postId, ClaimsHelper.GetUserId(User)));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("user/{profileUserId}")]
        public async Task<IActionResult> GetPostsByUser(
            Guid profileUserId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetPostsByUserQuery(profileUserId, ClaimsHelper.GetUserId(User), pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("comments")]
        public async Task<IActionResult> GetComments(
            [FromQuery] Guid postId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(new GetCommentsQuery(postId, pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        public class CreatePostRequest
        {
            public string Content { get; set; } = default!;
            public Visibility Visibility { get; set; } = Visibility.Public;
            public List<IFormFile>? Attachments { get; set; }
        }

        public class UpdatePostRequest
        {
            public Guid PostId { get; set; }
            public string Content { get; set; } = default!;
            public Visibility Visibility { get; set; }
        }
    }
}