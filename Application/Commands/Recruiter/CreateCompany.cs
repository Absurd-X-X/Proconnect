using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Recruiter
{
    public class CreateCompany
    {
        public record CreateCompanyCommand(
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
            string? LogoUrl,
            string? LogoPublicId
        ) : IRequest<Result<Guid>>;

        public class CreateCompanyHandler(
            ICompanyRepository companyRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<CreateCompanyCommand, Result<Guid>>
        {
            public async Task<Result<Guid>> Handle(
                CreateCompanyCommand request,
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

                var nameExists = await companyRepository.ExistsByNameAsync(request.Name);

                if (nameExists)
                {
                    return Result<Guid>.Failure("A company with this name already exists");
                }

                var company = new Company
                {
                    Name = request.Name,
                    Industry = request.Industry,
                    Description = request.Description,
                    Website = request.Website,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    CompanySize = request.CompanySize,
                    CompanyType = request.CompanyType,
                    FoundedYear = request.FoundedYear,
                    LogoUrl = request.LogoUrl,
                    LogoPublicId = request.LogoPublicId,
                    IsVerified = false,
                    CreatedBy = user.Email,
                    DateCreated = DateTime.UtcNow
                };

                await companyRepository.CreateAsync(company);

                if (existingProfile is not null)
                {
                    existingProfile.CompanyId = company.Id;
                    existingProfile.IsCompanyAdmin = true;
                    existingProfile.Status = RecruiterStatus.Active;
                    existingProfile.DateModified = DateTime.UtcNow;

                    recruiterProfileRepository.UpdateAsync(existingProfile);
                }
                else
                {
                    var recruiterProfile = new RecruiterProfile
                    {
                        UserId = user.Id,
                        CompanyId = company.Id,
                        IsCompanyAdmin = true,
                        Status = RecruiterStatus.Active,
                        CreatedBy = user.Email,
                        DateCreated = DateTime.UtcNow
                    };

                    await recruiterProfileRepository.CreateAsync(recruiterProfile);
                }

                await auditLogRepository.AddAsync(
                    new AuditLog
                    {
                        UserId = user.Id,
                        Action = "CreateCompany",
                        CreatedBy = user.Id.ToString(),
                        Description = $"User {user.UserName} created company: {company.Name}"
                    });

                await unitOfWork.SaveAsync();

                return Result<Guid>.Success(company.Id, "Company created successfully");
            }
        }
    }
}