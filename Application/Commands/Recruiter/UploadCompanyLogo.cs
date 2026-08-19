using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands
{
    public class UploadCompanyLogo
    {
        public record UploadCompanyLogoCommand(
            Guid RequestingUserId,
            IFormFile File
        ) : IRequest<Result<string>>;

        public class UploadCompanyLogoHandler(
            ICompanyRepository companyRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IFileStorage fileStorage,
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

                if (requestingProfile is null || requestingProfile.CompanyId is null)
                {
                    return Result<string>.Failure("You are not linked to a company");
                }

                if (!requestingProfile.IsCompanyAdmin)
                {
                    return Result<string>.Failure("Only a company admin can update the company logo");
                }

                var company = await companyRepository.GetByIdAsync(requestingProfile.CompanyId.Value);

                if (company is null)
                {
                    return Result<string>.Failure("Company not found");
                }

                if (request.File is null || request.File.Length == 0)
                {
                    return Result<string>.Failure("No file was uploaded");
                }

                if (!string.IsNullOrWhiteSpace(company.LogoPublicId))
                {
                    await fileStorage.DeleteAsync(company.LogoPublicId, cancellationToken);
                }

                var uploadResult = await fileStorage.UploadAsync(
                    request.File,
                    "proconnect/company-logos",
                    cancellationToken);

                company.LogoUrl = uploadResult.Url;
                company.LogoPublicId = uploadResult.PublicId;

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

                return Result<string>.Success(uploadResult.Url, "Logo updated successfully");
            }

            public record UploadCompanyLogoDto(IFormFile File);
        }
    }
}