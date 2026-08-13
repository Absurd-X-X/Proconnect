using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Recruiter
{
    public class CreateRecruiterProfile
    {
        public record CreateRecruiterProfileCommand(
            Guid UserId,
            Guid? CompanyId,
            string JobTitle,
            string Department,
            bool IsCompanyAdmin
        ) : IRequest<Result<string>>;



        public class CreateRecruiterProfileHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CreateRecruiterProfileCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                CreateRecruiterProfileCommand request,
                CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByIdAsync(request.UserId);

                if (user is null)
                {
                    return Result<string>.Failure("Invalid User In operation");
                }

                var recruiterProfile = new RecruiterProfile
                {
                    UserId = user.Id,

                    CompanyId = request.CompanyId,

                    JobTitle = request.JobTitle,

                    Department = request.Department,

                    IsCompanyAdmin = request.IsCompanyAdmin,

                    CreatedBy = request.UserId.ToString()
                };


                await recruiterProfileRepository
                    .CreateAsync(recruiterProfile);


                await unitOfWork.SaveAsync();


                return Result<string>.Success(
                    "Recruiter profile created successfully",
                    "created");
            }
        }
    }
}