using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Constant;
using Domain.Entities;
using MediatR;

namespace Application.Commands
{
    public class VerifyCompany
    {
        public record VerifyCompanyCommand(
            Guid RequestingUserId,
            Guid CompanyId
        ) : IRequest<Result<Guid>>;

        public class VerifyCompanyHandler(
            ICompanyRepository companyRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<VerifyCompanyCommand, Result<Guid>>
        {
            public async Task<Result<Guid>> Handle(
                VerifyCompanyCommand request,
                CancellationToken cancellationToken)
            {
                var requestingUser = await userRepository.GetByIdAsync(request.RequestingUserId);

                if (requestingUser is null || requestingUser.Role != Roles.Admin)
                {
                    return Result<Guid>.Failure("Only a platform admin can verify a company");
                }

                var company = await companyRepository.GetByIdAsync(request.CompanyId);

                if (company is null)
                {
                    return Result<Guid>.Failure("Company not found");
                }

                if (company.IsVerified)
                {
                    return Result<Guid>.Failure("Company is already verified");
                }

                company.IsVerified = true;
                company.VerifiedAt = DateTime.UtcNow;

                companyRepository.UpdateAsync(company);

                await auditLogRepository.AddAsync(
                    new AuditLog
                    {
                        UserId = requestingUser.Id,
                        Action = "VerifyCompany",
                        CreatedBy = requestingUser.Id.ToString(),
                        Description = $"Platform admin verified company: {company.Name}"
                    });

                await unitOfWork.SaveAsync();

                return Result<Guid>.Success(company.Id, "Company verified successfully");
            }
        }
    }
}