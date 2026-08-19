using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;
using System.Text.Json;

namespace Application.Commands
{
    public class UpdateCompanyProfile
    {
        public record CompanyLocationDto(
            string City,
            string? Address,
            bool IsHeadquarters
        );

        public record UpdateCompanyProfileCommand(
            Guid RequestingUserId,
            string Name,
            string Industry,
            string Description,
            string? Website,
            string Email,
            string PhoneNumber,
            string CompanySize,
            string CompanyType,
            int? FoundedYear,
            string? LinkedInUrl,
            string? TwitterUrl,
            string? FacebookUrl,
            string? InstagramUrl,
            List<CompanyLocationDto>? Locations
        ) : IRequest<Result<Guid>>;

        public class UpdateCompanyProfileHandler(
            ICompanyRepository companyRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<UpdateCompanyProfileCommand, Result<Guid>>
        {
            public async Task<Result<Guid>> Handle(
                UpdateCompanyProfileCommand request,
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
                    return Result<Guid>.Failure("Only a company admin can edit the company profile");
                }

                var company = await companyRepository.GetByIdAsync(requestingProfile.CompanyId.Value);

                if (company is null)
                {
                    return Result<Guid>.Failure("Company not found");
                }

                if (!string.Equals(company.Name, request.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var nameExists = await companyRepository.ExistsByNameAsync(request.Name);

                    if (nameExists)
                    {
                        return Result<Guid>.Failure("A company with this name already exists");
                    }
                }

                company.Name = request.Name;
                company.Industry = request.Industry;
                company.Description = request.Description;
                company.Website = request.Website;
                company.Email = request.Email;
                company.PhoneNumber = request.PhoneNumber;
                company.CompanySize = request.CompanySize;
                company.CompanyType = request.CompanyType;
                company.FoundedYear = request.FoundedYear;
                company.LinkedInUrl = request.LinkedInUrl;
                company.TwitterUrl = request.TwitterUrl;
                company.FacebookUrl = request.FacebookUrl;
                company.InstagramUrl = request.InstagramUrl;

                if (request.Locations is not null)
                {
                    company.Locations = JsonSerializer.Serialize(request.Locations);
                }

                companyRepository.UpdateAsync(company);

                await auditLogRepository.AddAsync(
                    new AuditLog
                    {
                        UserId = requestingProfile.UserId,
                        Action = "UpdateCompanyProfile",
                        CreatedBy = requestingProfile.UserId.ToString(),
                        Description = $"Admin updated company profile: {company.Name}"
                    });

                await unitOfWork.SaveAsync();

                return Result<Guid>.Success(company.Id, "Company profile updated successfully");
            }
        }
    }
}