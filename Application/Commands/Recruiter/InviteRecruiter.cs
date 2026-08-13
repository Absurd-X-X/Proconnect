using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;
using System.Security.Cryptography;

namespace Application.Commands.Recruiter
{
    public class InviteRecruiter
    {
        public record InviteRecruiterCommand(
            Guid RequestingUserId,
            Guid CompanyId
        ) : IRequest<Result<string>>;

        public class InviteRecruiterHandler(
            ICompanyRepository companyRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<InviteRecruiterCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                InviteRecruiterCommand request,
                CancellationToken cancellationToken)
            {
                var requestingProfile = await recruiterProfileRepository
                    .GetByUserIdAsync(request.RequestingUserId);

                if (requestingProfile is null || requestingProfile.CompanyId != request.CompanyId)
                {
                    return Result<string>.Failure("You are not a member of this company");
                }

                if (!requestingProfile.IsCompanyAdmin)
                {
                    return Result<string>.Failure("Only a company admin can invite recruiters");
                }

                var company = await companyRepository.GetByIdAsync(request.CompanyId);

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
                        Description = $"Admin generated a new invitation code for company: {company.Name}"
                    });

                await unitOfWork.SaveAsync();

                return Result<string>.Success(code, "Invitation code generated successfully");
            }
        }
    }
}