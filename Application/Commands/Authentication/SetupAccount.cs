using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Constant;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Authentication
{
    public class SetupAccount
    {
        public record SetupAccountCommand(
            string Email,
            string AccountType
        ) : IRequest<Result<string>>;


        public class SetupAccountHandler(
        IUserRepository userRepository,
        IProfessionalProfileRepository professionalProfileRepository,
        IRecruiterProfileRepository recruiterProfileRepository,
        IUnitOfWork unitOfWork)
    : IRequestHandler<SetupAccountCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                SetupAccountCommand request,
                CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByEmailAsync(request.Email);

                if (user is null)
                    return Result<string>.Failure("User not found");

                if (!user.IsVerified)
                    return Result<string>.Failure("Please verify your email first");

                if (user.Role != Roles.User)
                    return Result<string>.Failure("Account type already selected");

                switch (request.AccountType)
                {
                    case Roles.Professional:
                        user.Role = Roles.Professional;

                        var professionalProfile = new ProfessionalProfile
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            DateCreated = DateTime.UtcNow,
                            UserStatus = UserStatus.Active,
                            DateModified = DateTime.UtcNow,
                            CreatedBy = user.Email
                        };

                        await professionalProfileRepository.AddAsync(professionalProfile);
                        break;

                    case Roles.Recruiter:
                        user.Role = Roles.Recruiter;

                        var recruiterProfile = new RecruiterProfile
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            DateCreated = DateTime.UtcNow,
                            DateModified = DateTime.UtcNow,
                            CreatedBy = user.Email
                        };

                        await recruiterProfileRepository.CreateAsync(recruiterProfile);
                        break;

                    default:
                        return Result<string>.Failure("Invalid account type");
                }

                user.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success(
                    "Account setup completed successfully",
                    "completed");
            }
        }
    }
}