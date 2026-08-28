using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Social
{
    public class CreatePost
    {
        public record CreatePostCommand(
            Guid UserId,
            string Content,
            Visibility Visibility,
            List<IFormFile>? Attachments) : IRequest<Result<PostResponse>>;

        public class CreatePostHandler(
            IPostRepository postRepository,
            IFileUploadRepository fileUploadRepository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork) : IRequestHandler<CreatePostCommand, Result<PostResponse>>
        {
            public async Task<Result<PostResponse>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Content) && (request.Attachments is null || request.Attachments.Count == 0))
                {
                    return Result<PostResponse>.Failure("A post needs either text or an attachment");
                }

                var post = new Post
                {
                    UserId = request.UserId,
                    Content = request.Content ?? string.Empty,
                    Visibility = request.Visibility,
                    CreatedBy = request.UserId.ToString()
                };

                await postRepository.AddAsync(post);
                await unitOfWork.SaveAsync();

                var attachmentUrls = new List<string>();

                if (request.Attachments is not null)
                {
                    var order = 0;

                    foreach (var file in request.Attachments)
                    {
                        if (file.Length == 0) continue;

                        var uploadResult = await fileStorage.UploadAsync(file, "proconnect/post-attachments", cancellationToken);

                        var fileUpload = new FileUpload
                        {
                            UserId = request.UserId,
                            PostId = post.Id,
                            FileName = file.FileName,
                            FileUrl = uploadResult.Url,
                            FileType = file.ContentType,
                            FileSize = (int)file.Length,
                            DisplayOrder = order++,
                            CreatedBy = request.UserId.ToString()
                        };

                        await fileUploadRepository.AddAsync(fileUpload);
                        attachmentUrls.Add(uploadResult.Url);
                    }

                    await unitOfWork.SaveAsync();
                }

                return Result<PostResponse>.Success(
                    new PostResponse(post.Id, post.Content, post.Visibility, attachmentUrls, post.DateCreated),
                    "Post created successfully");
            }
        }
    }

    public record PostResponse(
        Guid Id,
        string Content,
        Visibility Visibility,
        List<string> AttachmentUrls,
        DateTime DateCreated);
}