using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Commands
{
    public class RemoveRecruiter
    {
        public record RemoveRecruiterCommand(
            Guid RequestingUserId,
            Guid RecruiterProfileId
        ) : IRequest<Result<Guid>>;

        public class RemoveRecruiterHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            ICompanyRepository companyRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository,
            IEmailService emailService,
            IOptions<AppSettings> appSettings)
            : IRequestHandler<RemoveRecruiterCommand, Result<Guid>>
        {
            private readonly AppSettings settings = appSettings.Value;

            public async Task<Result<Guid>> Handle(
                RemoveRecruiterCommand request,
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
                    return Result<Guid>.Failure("Only a company admin can approve or reject recruiters");
                }

                var targetProfile = await recruiterProfileRepository
                    .GetByIdAsync(request.RecruiterProfileId);

                if (targetProfile is null || targetProfile.CompanyId != requestingProfile.CompanyId)
                {
                    return Result<Guid>.Failure("Recruiter not found in your company");
                }

                if (targetProfile.Id == requestingProfile.Id)
                {
                    return Result<Guid>.Failure("You cannot remove yourself from the company");
                }

                var company = await companyRepository.GetByIdAsync(requestingProfile.CompanyId!.Value);

                targetProfile.CompanyId = null;
                targetProfile.IsCompanyAdmin = false;
                targetProfile.DateModified = DateTime.UtcNow;

                recruiterProfileRepository.UpdateAsync(targetProfile);

                await auditLogRepository.AddAsync(
                    new AuditLog
                    {
                        UserId = requestingProfile.UserId,
                        Action = "RemoveRecruiter",
                        CreatedBy = requestingProfile.UserId.ToString(),
                        Description = $"Admin removed recruiter {targetProfile.Id} from company"
                    });

                await unitOfWork.SaveAsync();

                if (company is not null)
                {
                    await emailService.SendEmailAsync(
                        new EmailRequest
                        {
                            To = targetProfile.User.Email,
                            Subject = $"You've been removed from {company.Name}",
                            Body = EmailTemplates.RecruiterRemovedEmail(
                                targetProfile.User.FirstName, company.Name, settings.FrontendUrl)
                        });
                }

                return Result<Guid>.Success(targetProfile.Id, "Recruiter removed from company");
            }
        }
    }
}