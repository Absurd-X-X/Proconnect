using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Messaging
{
    public class SendMessage
    {
        public record SendMessageCommand(
            Guid UserId,
            Guid ConversationId,
            string? Content,
            List<IFormFile>? Attachments) : IRequest<Result<MessageResponse>>;

        public class SendMessageHandler(
            IConversationParticipantRepository participantRepository,
            IConversationRepository conversationRepository,
            IMessageRepository messageRepository,
            IFileUploadRepository fileUploadRepository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork) : IRequestHandler<SendMessageCommand, Result<MessageResponse>>
        {
            public async Task<Result<MessageResponse>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Content) && (request.Attachments is null || request.Attachments.Count == 0))
                {
                    return Result<MessageResponse>.Failure("A message needs either text or an attachment");
                }

                var participant = await participantRepository.GetByConversationAndUserAsync(request.ConversationId, request.UserId);

                if (participant is null)
                {
                    return Result<MessageResponse>.Failure("You're not a participant of this conversation");
                }

                var message = new Message
                {
                    ConversationId = request.ConversationId,
                    UserId = request.UserId,
                    Content = request.Content,
                    CreatedBy = request.UserId.ToString()
                };

                await messageRepository.AddAsync(message);
                await unitOfWork.SaveAsync();

                var attachmentUrls = new List<string>();

                if (request.Attachments is not null)
                {
                    var order = 0;

                    foreach (var file in request.Attachments)
                    {
                        if (file.Length == 0) continue;

                        var uploadResult = await fileStorage.UploadAsync(file, "proconnect/message-attachments", cancellationToken);

                        await fileUploadRepository.AddAsync(new FileUpload
                        {
                            UserId = request.UserId,
                            MessageId = message.Id,
                            FileName = file.FileName,
                            FileUrl = uploadResult.Url,
                            FileType = file.ContentType,
                            FileSize = (int)file.Length,
                            DisplayOrder = order++,
                            CreatedBy = request.UserId.ToString()
                        });

                        attachmentUrls.Add(uploadResult.Url);
                    }

                    await unitOfWork.SaveAsync();
                }

                var conversation = await conversationRepository.GetByIdAsync(request.ConversationId);

                if (conversation is not null)
                {
                    conversation.DateModified = DateTime.UtcNow;
                    conversationRepository.Update(conversation);
                    await unitOfWork.SaveAsync();
                }

                return Result<MessageResponse>.Success(
                    new MessageResponse(message.Id, message.ConversationId, message.Content, attachmentUrls, message.DateCreated),
                    "Message sent");
            }
        }
    }

    public record MessageResponse(Guid Id, Guid ConversationId, string? Content, List<string> AttachmentUrls, DateTime DateCreated);
}