using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands
{
    public class UploadCompanyLogo
    {
        public record UploadCompanyLogoCommand(
            Guid RequestingUserId,
            Guid CompanyId,
            string LogoUrl,
            string LogoPublicId
        ) : IRequest<Result<string>>;

        public class UploadCompanyLogoHandler(
            ICompanyRepository companyRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<UploadCompanyLogoCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                UploadCompanyLogoCommand request,
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
                    return Result<string>.Failure("Only a company admin can update the company logo");
                }

                var company = await companyRepository.GetByIdAsync(request.CompanyId);

                if (company is null)
                {
                    return Result<string>.Failure("Company not found");
                }

                company.LogoUrl = request.LogoUrl;
                company.LogoPublicId = request.LogoPublicId;

                companyRepository.UpdateAsync(company);

                await auditLogRepository.AddAsync(
                    new AuditLog
                    {
                        UserId = requestingProfile.UserId,
                        Action = "UploadCompanyLogo",
                        CreatedBy = requestingProfile.UserId.ToString(),
                        Description = $"Admin updated company logo: {company.Name}"
                    });

                await unitOfWork.SaveAsync();

                return Result<string>.Success(company.LogoUrl, "Logo updated successfully");
            }
        }
    }
}