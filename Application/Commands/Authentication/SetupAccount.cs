using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Constant;
using Application.Contract.Settings;
using Application.Services.Interfaces;
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
        ) : IRequest<Result<LoginResponse>>;


        public class SetupAccountHandler(
        IUserRepository userRepository,
        IProfessionalProfileRepository professionalProfileRepository,
        IRecruiterProfileRepository recruiterProfileRepository,
        ITokenServices tokenServices,
        IUnitOfWork unitOfWork)
    : IRequestHandler<SetupAccountCommand, Result<LoginResponse>>
        {
            public async Task<Result<LoginResponse>> Handle(
                SetupAccountCommand request,
                CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByEmailAsync(request.Email);

                if (user is null)
                    return Result<LoginResponse>.Failure("User not found");

                if (!user.IsVerified)
                    return Result<LoginResponse>.Failure("Please verify your email first");

                if (user.Role != Roles.User)
                    return Result<LoginResponse>.Failure("Account type already selected");

                Guid profileId;

                switch (request.AccountType)
                {
                    case Roles.Professional:
                        user.Role = Roles.Professional;

                        var professionalProfile = new ProfessionalProfile
                        {
                            UserId = user.Id,
                            DateCreated = DateTime.UtcNow,
                            UserStatus = UserStatus.Active,
                            DateModified = DateTime.UtcNow,
                            CreatedBy = user.Email,
                        };

                        profileId = professionalProfile.Id;

                        await professionalProfileRepository.AddAsync(professionalProfile);
                        break;

                    case Roles.Recruiter:
                        user.Role = Roles.Recruiter;

                        var recruiterProfile = new RecruiterProfile
                        {
                            UserId = user.Id,
                            DateCreated = DateTime.UtcNow,
                            DateModified = DateTime.UtcNow,
                            CreatedBy = user.Email
                        };

                        profileId = recruiterProfile.Id;

                        await recruiterProfileRepository.CreateAsync(recruiterProfile);
                        break;

                    default:
                        return Result<LoginResponse>.Failure("Invalid account type");
                }

                user.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                var response = new LoginResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role,
                    ProfileId = profileId.ToString(),
                    UserName = user.UserName
                };

                var token = tokenServices.GenerateToken(response);


                return Result<LoginResponse>.Success(
                    response,
                    token);
            }
        }
    }
}