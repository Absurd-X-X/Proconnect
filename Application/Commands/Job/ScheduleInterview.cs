using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Commands.Job
{
    public class ScheduleInterview
    {
        public record ScheduleInterviewCommand(
            Guid ApplicationId,
            Guid RecruiterProfileId,
            DateTime InterviewScheduledAt,
            string InterviewType,
            string InterviewLocationOrLink
        ) : IRequest<Result<string>>;

        public class ScheduleInterviewHandler(
            IJobApplicationRepository jobApplicationRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IEmailService emailService,
            INotificationService notificationService,
            IOptions<AppSettings> appSettings,
            IUnitOfWork unitOfWork)
            : IRequestHandler<ScheduleInterviewCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                ScheduleInterviewCommand request,
                CancellationToken cancellationToken)
            {
                var application = await jobApplicationRepository.GetByIdAsync(request.ApplicationId);

                if (application is null)
                    return Result<string>.Failure("Application not found");

                var callingRecruiter = await recruiterProfileRepository.GetByIdAsync(request.RecruiterProfileId);

                if (callingRecruiter is null || callingRecruiter.Status != RecruiterStatus.Active)
                    return Result<string>.Failure("You are not authorized to schedule an interview for this application");

                if (callingRecruiter.CompanyId != application.Job.CompanyId)
                    return Result<string>.Failure("You are not authorized to schedule an interview for this application");

                if (application.JobStatus == JobStatus.Withdrawn)
                    return Result<string>.Failure("This application was withdrawn by the candidate and can no longer be updated");

                if (request.InterviewScheduledAt <= DateTime.UtcNow)
                    return Result<string>.Failure("Interview date must be in the future");

                application.InterviewScheduledAt = request.InterviewScheduledAt;

                application.InterviewType = request.InterviewType;

                application.InterviewLocationOrLink = request.InterviewLocationOrLink;

                application.JobStatus = JobStatus.Interview;

                application.UpdatedAt = DateTime.UtcNow;

                application.LastActionedByRecruiterProfileId = request.RecruiterProfileId;

                await unitOfWork.SaveAsync();

                var applicantEmail = application.ProfessionalProfile.User.Email;
                var applicantName = $"{application.ProfessionalProfile.User.FirstName} {application.ProfessionalProfile.User.LastName}".Trim();

                if (!string.IsNullOrWhiteSpace(applicantEmail))
                {
                    var emailBody = EmailTemplates.InterviewScheduledEmail(
                        applicantName,
                        application.Job.Title,
                        application.Job.Company.Name,
                        request.InterviewScheduledAt,
                        request.InterviewType,
                        request.InterviewLocationOrLink,
                        appSettings.Value.FrontendUrl);

                    await emailService.SendEmailAsync(new EmailRequest
                    {
                        To = applicantEmail,
                        Subject = "Interview Scheduled — ProConnect",
                        Body = emailBody
                    });
                }

                await notificationService.SendNotificationAsync(
                    recipientUserId: application.ProfessionalProfile.UserId,
                    actorUserId: null,
                    actorName: application.Job.Company.Name,
                    actorAvatarUrl: application.Job.Company.LogoUrl,
                    title: "Interview scheduled",
                    message: $"An interview has been scheduled for your application to \"{application.Job.Title}\" on {request.InterviewScheduledAt:MMM dd, yyyy 'at' h:mm tt}",
                    type: NotificationType.JobStatusUpdate,
                    sourceEntityType: NotificationSourceEntityType.JobApplication,
                    sourceEntityId: application.Id,
                    actionUrl: $"/my-applications.html?id={application.Id}",
                    createdBy: request.RecruiterProfileId.ToString());

                return Result<string>.Success(application.Id.ToString(), "Interview scheduled successfully");
            }
        }
    }
}