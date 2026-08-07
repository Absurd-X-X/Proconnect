using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands
{
    public class AddEducation
    {
        public record AddEducationCommand(
            Guid ProfessionalProfileId,
            string Institution,
            string Degree,
            string FieldOfStudy,
            DateTime StartDate,
            DateTime EndDate,
            string Grade,
            string Description
        ) : IRequest<Result<string>>;

        public class AddEducationHandler(
            IEducationRepository educationRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<
                AddEducation.AddEducationCommand,
                Result<string>>
        {
            public async Task<Result<string>> Handle(
                AddEducation.AddEducationCommand request,
                CancellationToken cancellationToken)
            {
                var education = new Education
                {
                    ProfessionalProfileId = request.ProfessionalProfileId,

                    Institution = request.Institution,

                    Degree = request.Degree,

                    FieldOfStudy = request.FieldOfStudy,

                    StartDate = request.StartDate,

                    EndDate = request.EndDate,

                    Grade = request.Grade,

                    Description = request.Description,

                    DateCreated = DateTime.UtcNow
                };


                await educationRepository.AddAsync(education);

                await unitOfWork.SaveAsync();


                return Result<string>.Success(
                    "Education added successfully",
                    "created");
            }
        }
    }
}