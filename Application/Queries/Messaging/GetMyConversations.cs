using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Messaging
{
    public class GetMyConversations
    {
        public record GetMyConversationsQuery(Guid UserId, PageRequest PageRequest, bool UsePaging)
            : IRequest<Result<PageResponse<ConversationListItemResponse>>>;

        public class GetMyConversationsHandler(
            IConversationRepository conversationRepository,
            IMessageRepository messageRepository)
            : IRequestHandler<GetMyConversationsQuery, Result<PageResponse<ConversationListItemResponse>>>
        {
            public async Task<Result<PageResponse<ConversationListItemResponse>>> Handle(GetMyConversationsQuery request, CancellationToken cancellationToken)
            {
                var page = await conversationRepository.GetUserConversationsAsync(request.UserId, request.PageRequest, request.UsePaging);

                var items = new List<ConversationListItemResponse>();

                foreach (var conversation in page.Items)
                {
                    var myParticipant = conversation.Participants.FirstOrDefault(p => p.UserId == request.UserId);

                    var unreadCount = await messageRepository.GetUnreadCountAsync(
                        conversation.Id, request.UserId, myParticipant?.LastReadAt);

                    var lastMessagePage = await messageRepository.GetByConversationIdAsync(
                        conversation.Id, new PageRequest { PageNumber = 1, PageSize = 1 }, true);

                    var lastMessage = lastMessagePage.Items.FirstOrDefault();

                    string? displayTitle = conversation.Title;
                    string? displayPhotoUrl = conversation.GroupPhotoUrl;

                    if (!conversation.IsGroup)
                    {
                        var otherParticipant = conversation.Participants.FirstOrDefault(p => p.UserId != request.UserId);
                        displayTitle = otherParticipant is not null
                            ? $"{otherParticipant.User.FirstName} {otherParticipant.User.LastName}"
                            : "Conversation";
                        displayPhotoUrl = otherParticipant?.User.ProfilePictureUrl;
                    }

                    items.Add(new ConversationListItemResponse(
                        conversation.Id,
                        conversation.IsGroup,
                        displayTitle,
                        displayPhotoUrl,
                        lastMessage?.Content,
                        lastMessage?.User.FirstName,
                        lastMessage?.DateCreated ?? conversation.DateCreated,
                        unreadCount));
                }

                var response = new PageResponse<ConversationListItemResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<ConversationListItemResponse>>.Success(response, "Conversations retrieved successfully");
            }
        }
    }

    public record ConversationListItemResponse(
        Guid Id,
        bool IsGroup,
        string? Title,
        string? PhotoUrl,
        string? LastMessagePreview,
        string? LastMessageSenderFirstName,
        DateTime LastActivityAt,
        int UnreadCount);
}