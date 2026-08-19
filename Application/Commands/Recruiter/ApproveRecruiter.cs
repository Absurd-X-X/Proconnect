using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Commands
{
    public class ApproveRecruiter
    {
        public record ApproveRecruiterCommand(
            Guid RequestingUserId,
            Guid RecruiterProfileId,
            bool Approve
        ) : IRequest<Result<Guid>>;

        public class ApproveRecruiterHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            ICompanyRepository companyRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository,
            IEmailService emailService,
            IOptions<AppSettings> appSettings)
            : IRequestHandler<ApproveRecruiterCommand, Result<Guid>>
        {
            private readonly AppSettings settings = appSettings.Value;

            public async Task<Result<Guid>> Handle(
                ApproveRecruiterCommand request,
                CancellationToken cancellationToken)
            {
                var requestingProfile = await recruiterProfileRepository
                    .GetByUserIdAsync(request.RequestingUserId);

                if (requestingProfile is null || requestingProfile.CompanyId is null)
                {
                    return Result<Guid>.Failure("You are not linked to a company");
                }

                if (!requestingProfile.IsCompanyAdmin)
                {
                    return Result<Guid>.Failure("Only a company admin can remove recruiters");
                }

                var targetProfile = await recruiterProfileRepository
                    .GetByIdAsync(request.RecruiterProfileId);

                if (targetProfile is null)
                {
                    return Result<Guid>.Failure("Recruiter request not found");
                }

                if (targetProfile.CompanyId != requestingProfile.CompanyId)
                {
                    return Result<Guid>.Failure("This recruiter does not belong to your company");
                }

                if (targetProfile.Status != RecruiterStatus.Pending)
                {
                    return Result<Guid>.Failure("This request has already been processed");
                }

                var company = await companyRepository.GetByIdAsync(requestingProfile.CompanyId!.Value);

                if (request.Approve)
                {
                    targetProfile.Status = RecruiterStatus.Active;
                    targetProfile.DateModified = DateTime.UtcNow;

                    recruiterProfileRepository.UpdateAsync(targetProfile);

                    await auditLogRepository.AddAsync(
                        new AuditLog
                        {
                            UserId = requestingProfile.UserId,
                            Action = "ApproveRecruiter",
                            CreatedBy = requestingProfile.UserId.ToString(),
                            Description = $"Admin approved recruiter join request: {targetProfile.Id}"
                        });
                }
                else
                {
                    targetProfile.CompanyId = null;
                    targetProfile.Status = RecruiterStatus.Pending;
                    targetProfile.DateModified = DateTime.UtcNow;

                    recruiterProfileRepository.UpdateAsync(targetProfile);

                    await auditLogRepository.AddAsync(
                        new AuditLog
                        {
                            UserId = requestingProfile.UserId,
                            Action = "RejectRecruiter",
                            CreatedBy = requestingProfile.UserId.ToString(),
                            Description = $"Admin rejected recruiter join request: {targetProfile.Id}"
                        });
                }

                await unitOfWork.SaveAsync();

                if (company is not null)
                {
                    var emailBody = request.Approve
                        ? EmailTemplates.RecruiterApprovedEmail(
                            targetProfile.User.FirstName, company.Name, settings.FrontendUrl)
                        : EmailTemplates.RecruiterRejectedEmail(
                            targetProfile.User.FirstName, company.Name, settings.FrontendUrl);

                    await emailService.SendEmailAsync(
                        new EmailRequest
                        {
                            To = targetProfile.User.Email,
                            Subject = request.Approve
                                ? $"You've been approved to join {company.Name}"
                                : $"Update on your request to join {company.Name}",
                            Body = emailBody
                        });
                }

                return Result<Guid>.Success(
                    targetProfile.Id,
                    request.Approve ? "Recruiter approved" : "Recruiter request rejected");
            }
        }
    }
}