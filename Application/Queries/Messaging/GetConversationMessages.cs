using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Messaging
{
    public class GetConversationMessages
    {
        public record GetConversationMessagesQuery(Guid UserId, Guid ConversationId, PageRequest PageRequest, bool UsePaging)
            : IRequest<Result<PageResponse<MessageItemResponse>>>;

        public class GetConversationMessagesHandler(
            IConversationParticipantRepository participantRepository,
            IMessageRepository messageRepository,
            IFileUploadRepository fileUploadRepository)
            : IRequestHandler<GetConversationMessagesQuery, Result<PageResponse<MessageItemResponse>>>
        {
            public async Task<Result<PageResponse<MessageItemResponse>>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
            {
                var participant = await participantRepository.GetByConversationAndUserAsync(request.ConversationId, request.UserId);

                if (participant is null)
                {
                    return Result<PageResponse<MessageItemResponse>>.Failure("You're not a participant of this conversation");
                }

                var page = await messageRepository.GetByConversationIdAsync(request.ConversationId, request.PageRequest, request.UsePaging);

                var items = new List<MessageItemResponse>();

                foreach (var message in page.Items)
                {
                    var attachments = await fileUploadRepository.GetByMessageIdAsync(message.Id);

                    items.Add(new MessageItemResponse(
                        message.Id,
                        message.UserId,
                        message.User.FirstName,
                        message.User.LastName,
                        message.User.ProfilePictureUrl,
                        message.Content,
                        attachments.Select(a => a.FileUrl).ToList(),
                        message.DateCreated));
                }

                var response = new PageResponse<MessageItemResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<MessageItemResponse>>.Success(response, "Messages retrieved successfully");
            }
        }
    }

    public record MessageItemResponse(
        Guid Id,
        Guid SenderId,
        string SenderFirstName,
        string SenderLastName,
        string? SenderProfilePictureUrl,
        string? Content,
        List<string> AttachmentUrls,
        DateTime DateCreated);
}