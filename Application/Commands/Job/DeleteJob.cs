using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Job
{
    public class DeleteJob
    {
        public record DeleteJobCommand(
            Guid JobId,
            Guid RecruiterProfileId
        ) : IRequest<Result<string>>;

        public class DeleteJobHandler(
            IJobRepository jobRepository,
            IJobApplicationRepository jobApplicationRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
            : IRequestHandler<DeleteJobCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                DeleteJobCommand request,
                CancellationToken cancellationToken)
            {
                var job = await jobRepository.GetByIdAsync(request.JobId);

                if (job is null)
                    return Result<string>.Failure("Job not found");

                if (job.RecruiterProfileId != request.RecruiterProfileId)
                    return Result<string>.Failure("You are not authorized to delete this job");

                var applicationsPage = await jobApplicationRepository.GetByJobIdAsync(
                    request.JobId, new PageRequest(), usePaging: false);

                var jobTitle = job.Title;
                var companyName = job.Company.Name;
                var companyLogo = job.Company.LogoUrl;

                jobRepository.Delete(job);

                await unitOfWork.SaveAsync();

                foreach (var application in applicationsPage.Items)
                {
                    await notificationService.SendNotificationAsync(
                        recipientUserId: application.ProfessionalProfile.UserId,
                        actorUserId: null,
                        actorName: companyName,
                        actorAvatarUrl: companyLogo,
                        title: "Job posting removed",
                        message: $"The job \"{jobTitle}\" you applied to has been removed by the recruiter",
                        type: NotificationType.JobStatusUpdate,
                        sourceEntityType: NotificationSourceEntityType.JobApplication,
                        sourceEntityId: application.Id,
                        actionUrl: "/my-applications.html",
                        createdBy: request.RecruiterProfileId.ToString());
                }

                return Result<string>.Success(job.Id.ToString(), "Job deleted successfully");
            }
        }
    }
}