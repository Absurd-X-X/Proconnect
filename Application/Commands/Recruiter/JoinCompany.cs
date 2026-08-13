using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Recruiter
{
    public class JoinCompany
    {
        public record JoinCompanyCommand(
            Guid RequestingUserId,
            string InvitationCode
        ) : IRequest<Result<Guid>>;

        public class JoinCompanyHandler(
            ICompanyRepository companyRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<JoinCompanyCommand, Result<Guid>>
        {
            public async Task<Result<Guid>> Handle(
                JoinCompanyCommand request,
                CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByIdAsync(request.RequestingUserId);

                if (user is null)
                {
                    return Result<Guid>.Failure("User not found");
                }

                var existingProfile = await recruiterProfileRepository
                    .GetByUserIdAsync(request.RequestingUserId);

                if (existingProfile is not null && existingProfile.CompanyId is not null)
                {
                    return Result<Guid>.Failure("You are already linked to a company");
                }

                var company = await companyRepository
                    .GetByInvitationCodeAsync(request.InvitationCode);

                if (company is null)
                {
                    return Result<Guid>.Failure("Invalid invitation code");
                }

                if (company.InvitationCodeExpiry is null ||
                    company.InvitationCodeExpiry < DateTime.UtcNow)
                {
                    return Result<Guid>.Failure("This invitation code has expired");
                }

                if (existingProfile is not null)
                {
                    existingProfile.CompanyId = company.Id;
                    existingProfile.IsCompanyAdmin = false;
                    existingProfile.Status = RecruiterStatus.Pending;
                    existingProfile.DateModified = DateTime.UtcNow;

                    recruiterProfileRepository.UpdateAsync(existingProfile);
                }
                else
                {
                    existingProfile = new RecruiterProfile
                    {
                        UserId = user.Id,
                        CompanyId = company.Id,
                        IsCompanyAdmin = false,
                        Status = RecruiterStatus.Pending,
                        CreatedBy = user.Email,
                        DateCreated = DateTime.UtcNow
                    };

                    await recruiterProfileRepository.CreateAsync(existingProfile);
                }

                await auditLogRepository.AddAsync(
                    new AuditLog
                    {
                        UserId = user.Id,
                        Action = "JoinCompanyRequest",
                        CreatedBy = user.Id.ToString(),
                        Description = $"User {user.UserName} requested to join company: {company.Name}"
                    });

                await unitOfWork.SaveAsync();

                return Result<Guid>.Success(
                    existingProfile.Id,
                    "Request submitted. Your company admin will review and approve your access.");
            }
        }
    }
}