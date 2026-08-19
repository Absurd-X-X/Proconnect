using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Application.Commands
{
    public class InviteRecruiter
    {
        public record InviteRecruiterCommand(
            Guid RequestingUserId,
            string RecruiterEmail
        ) : IRequest<Result<string>>;

        public class InviteRecruiterHandler(
            ICompanyRepository companyRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository,
            IEmailService emailService,
            IOptions<AppSettings> appSettings)
            : IRequestHandler<InviteRecruiterCommand, Result<string>>
        {
            private readonly AppSettings settings = appSettings.Value;

            public async Task<Result<string>> Handle(
                InviteRecruiterCommand request,
                CancellationToken cancellationToken)
            {
                var requestingProfile = await recruiterProfileRepository
                    .GetByUserIdAsync(request.RequestingUserId);

                if (requestingProfile is null || requestingProfile.CompanyId is null)
                {
                    return Result<string>.Failure("You are not linked to a company");
                }

                if (!requestingProfile.IsCompanyAdmin)
                {
                    return Result<string>.Failure("Only a company admin can invite recruiters");
                }

                var company = await companyRepository.GetByIdAsync(requestingProfile.CompanyId!.Value);

                if (company is null)
                {
                    return Result<string>.Failure("Company not found");
                }

                var code = RandomNumberGenerator
                    .GetInt32(0, 999999999)
                    .ToString("D9");

                company.InvitationCode = code;
                company.InvitationCodeExpiry = DateTime.UtcNow.AddDays(7);

                companyRepository.UpdateAsync(company);

                await auditLogRepository.AddAsync(
                    new AuditLog
                    {
                        UserId = requestingProfile.UserId,
                        Action = "InviteRecruiter",
                        CreatedBy = requestingProfile.UserId.ToString(),
                        Description = $"Admin generated an invitation code for {request.RecruiterEmail} to join company: {company.Name}"
                    });

                await unitOfWork.SaveAsync();

                var invitee = await userRepository.GetByEmailAsync(request.RecruiterEmail);
                var displayName = invitee is not null ? invitee.FirstName : request.RecruiterEmail;

                var baseUrl = settings.FrontendUrl.TrimEnd('/');
                var invitationLink =
                    $"{baseUrl}/join-company.html?code={Uri.EscapeDataString(code)}";

                var emailResult = await emailService.SendEmailAsync(
                    new EmailRequest
                    {
                        To = request.RecruiterEmail,
                        Subject = $"You've been invited to join {company.Name} on ProConnect",
                        Body = EmailTemplates.CompanyInvitationEmail(
                            displayName,
                            company.Name,
                            invitationLink)
                    });

                if (!emailResult.Status)
                {
                    return Result<string>.Failure(
                        $"Invitation code generated but email could not be sent. Please share the code manually: {code}. {emailResult.Message}");
                }

                return Result<string>.Success(code, "Invitation sent successfully");
            }
        }
    }
}