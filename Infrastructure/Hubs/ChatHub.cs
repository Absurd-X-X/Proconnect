using System.Collections.Concurrent;
using System.Security.Claims;
using Application.Common.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Hubs
{
    [Authorize]
    public class ChatHub(
        IConversationParticipantRepository participantRepository,
        IMessageRepository messageRepository,
        IFileUploadRepository fileUploadRepository) : Hub
    {
        private static readonly ConcurrentDictionary<string, int> OnlineConnectionCounts = new();

        private string? GetUserId() => Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();

            if (userId is not null)
            {
                var newCount = OnlineConnectionCounts.AddOrUpdate(userId, 1, (_, count) => count + 1);

                if (newCount == 1 && Guid.TryParse(userId, out var userGuid))
                {
                    var relatedUserIds = await participantRepository.GetRelatedUserIdsAsync(userGuid);
                    await Clients.Users(relatedUserIds.Select(id => id.ToString())).SendAsync("UserOnline", userId);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();

            if (userId is not null)
            {
                var newCount = OnlineConnectionCounts.AddOrUpdate(userId, 0, (_, count) => Math.Max(0, count - 1));

                if (newCount == 0)
                {
                    OnlineConnectionCounts.TryRemove(userId, out _);

                    if (Guid.TryParse(userId, out var userGuid))
                    {
                        var relatedUserIds = await participantRepository.GetRelatedUserIdsAsync(userGuid);
                        await Clients.Users(relatedUserIds.Select(id => id.ToString())).SendAsync("UserOffline", userId);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Join a specific conversation room — only if the caller is
        // actually a participant. Without this check, any authenticated
        // user could join and listen to any conversation's broadcasts.
        public async Task JoinConversation(string conversationId)
        {
            var userId = GetUserId();

            if (userId is null || !Guid.TryParse(userId, out var userGuid) || !Guid.TryParse(conversationId, out var conversationGuid))
            {
                return;
            }

            var participant = await participantRepository.GetByConversationAndUserAsync(conversationGuid, userGuid);

            if (participant is null)
            {
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        }

        public async Task Typing(string conversationId)
        {
            var userId = GetUserId();
            if (userId is null) return;

            await Clients.OthersInGroup($"conversation_{conversationId}").SendAsync("UserTyping", conversationId, userId);
        }

        public async Task StopTyping(string conversationId)
        {
            var userId = GetUserId();
            if (userId is null) return;

            await Clients.OthersInGroup($"conversation_{conversationId}").SendAsync("UserStoppedTyping", conversationId, userId);
        }

        // Called by the client AFTER a successful REST call to
        // /api/Messages/send-message — this does NOT persist anything
        // itself. It re-fetches the already-saved message from the DB
        // (so the broadcast reflects trusted server data, not whatever
        // the client claims) and relays it to everyone else in the room.
        public async Task NotifyNewMessage(string conversationId, string messageId)
        {
            var userId = GetUserId();

            if (userId is null
                || !Guid.TryParse(userId, out var userGuid)
                || !Guid.TryParse(conversationId, out var conversationGuid)
                || !Guid.TryParse(messageId, out var messageGuid))
            {
                return;
            }

            var participant = await participantRepository.GetByConversationAndUserAsync(conversationGuid, userGuid);
            if (participant is null) return;

            var message = await messageRepository.GetByIdAsync(messageGuid);
            if (message is null || message.ConversationId != conversationGuid) return;

            var attachments = await fileUploadRepository.GetByMessageIdAsync(messageGuid);

            var payload = new
            {
                id = message.Id,
                conversationId = message.ConversationId,
                senderId = message.UserId,
                senderFirstName = message.User.FirstName,
                senderLastName = message.User.LastName,
                senderProfilePictureUrl = message.User.ProfilePictureUrl,
                content = message.Content,
                attachmentUrls = attachments.Select(a => a.FileUrl).ToList(),
                dateCreated = message.DateCreated
            };

            await Clients.OthersInGroup($"conversation_{conversationId}").SendAsync("ReceiveMessage", payload);
        }

        // Called by the client right after connecting, so it can show accurate
        // presence immediately instead of waiting for a future UserOnline/
        // UserOffline event. Only checks the users passed in (the caller's
        // conversation partners), not everyone on the platform.
        public Task<List<string>> GetOnlineStatus(List<string> userIds)
        {
            var online = userIds.Where(id => OnlineConnectionCounts.ContainsKey(id)).ToList();
            return Task.FromResult(online);
        }
    }
}